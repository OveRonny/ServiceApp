using Microsoft.AspNetCore.Identity;
using serviceApp.Server.Data;
using serviceApp.Server.Features.Emails;

namespace serviceApp.Server.Features.FoodStorage;

public sealed class FrozenFoodNotificationService(
    IServiceScopeFactory scopeFactory,
    ILogger<FrozenFoodNotificationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromHours(24));
        do
        {
            await SendNotificationsAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task SendNotificationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var email = scope.ServiceProvider.GetRequiredService<ISmtpEmailSender>();
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var items = await db.FoodStockItems.IgnoreQueryFilters()
                .Include(x => x.FoodProduct)
                .Where(x => x.Quantity > 0 && x.FrozenDate != null &&
                    (x.FrozenOneYearNotificationSentAt == null || x.FrozenTwoYearNotificationSentAt == null))
                .ToListAsync(cancellationToken);

            foreach (var familyItems in items.GroupBy(x => x.FamilyId))
            {
                var recipients = await users.Users
                    .Where(x => x.FamilyId == familyItems.Key && x.EmailConfirmed && x.Email != null)
                    .Select(x => x.Email!)
                    .ToListAsync(cancellationToken);
                if (recipients.Count == 0) continue;

                var twoYear = familyItems.Where(x => x.FrozenDate!.Value.AddYears(2) <= today &&
                    x.FrozenTwoYearNotificationSentAt == null).ToList();
                var oneYear = familyItems.Where(x => x.FrozenDate!.Value.AddYears(1) <= today &&
                    x.FrozenOneYearNotificationSentAt == null && !twoYear.Contains(x)).ToList();

                foreach (var recipient in recipients)
                {
                    if (oneYear.Count > 0)
                        await email.SendAsync(recipient, "Frysevarer har passert ett år",
                            BuildBody("Disse frysevarene har vært fryst i minst ett år og bør prioriteres:", oneYear), cancellationToken);
                    if (twoYear.Count > 0)
                        await email.SendAsync(recipient, "Frysevarer har nådd toårsgrensen",
                            BuildBody("Disse frysevarene har nådd eller passert anbefalt maksgrense på to år:", twoYear), cancellationToken);
                }

                var sentAt = DateTimeOffset.UtcNow;
                oneYear.ForEach(x => x.FrozenOneYearNotificationSentAt = sentAt);
                twoYear.ForEach(x => { x.FrozenOneYearNotificationSentAt ??= sentAt; x.FrozenTwoYearNotificationSentAt = sentAt; });
                await db.SaveChangesAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kunne ikke sende varsler om frysevarer");
        }
    }

    private static string BuildBody(string introduction, IEnumerable<Entities.FoodStorage.FoodStockItem> items) =>
        $"<p>{introduction}</p><ul>{string.Join(string.Empty, items.Select(x =>
            $"<li>{System.Net.WebUtility.HtmlEncode(x.FoodProduct.Name)} – fryst {x.FrozenDate:dd.MM.yyyy}</li>"))}</ul><p>Åpne Matlager for å se og oppdatere varene.</p>";
}
