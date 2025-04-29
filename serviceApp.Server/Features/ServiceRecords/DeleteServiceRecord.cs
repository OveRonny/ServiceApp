namespace serviceApp.Server.Features.ServiceRecords;

public static class DeleteServiceRecord
{
    public record Command(int Id) : ICommand<bool>;
    public class Handler(ApplicationDbContext context) : ICommandHandler<Command, bool>
    {
        private readonly ApplicationDbContext context = context;
        public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
        {
            var serviceRecord = await context.ServiceRecords
                .Include(s => s.MileageHistory)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            if (serviceRecord == null)
            {
                return Result.Fail<bool>($"ServiceRecord with ID {request.Id} not found.");
            }

            if (serviceRecord.MileageHistory != null)
            {
                context.MileageHistories.Remove(serviceRecord.MileageHistory);
            }

            context.ServiceRecords.Remove(serviceRecord);
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    public class EndPoint : IEndpointDefinition
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapDelete("api/service-record/{id}", async (ISender sender, int id, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new Command(id), cancellationToken);
                if (result.Failure)
                {
                    return false;
                }
                return true;
            });
        }
    }
}


