namespace Arda9UserApi.Core.CQRS;

public interface ICommand<TResponse>
{
}

public interface ICommandHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}