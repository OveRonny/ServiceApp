namespace serviceApp.Server.Abstractions.RequestHandling;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
