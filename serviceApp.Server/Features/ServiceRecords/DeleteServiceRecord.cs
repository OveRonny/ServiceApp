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
}

[ApiController]
[Route("api/service-record")]
public class DeleteServiceRecordController(ISender sender) : ControllerBase
{
    private readonly ISender sender = sender;

    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteServiceRecord(int id)
    {
        var result = await sender.Send(new DeleteServiceRecord.Command(id));
        if (result.Failure)
        {
            return NotFound(result.Error);
        }
        return true;
    }
}
