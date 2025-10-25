namespace Arda9UserApi.Core.CQRS;

public interface IQuery<TResponse>
{
}

public interface IQueryHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> where TResponse : class
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}