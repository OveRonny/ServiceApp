
namespace serviceApp.Server.Features.Emails;

public interface ISmtpEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
}