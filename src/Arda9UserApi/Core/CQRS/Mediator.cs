using System.Collections.Concurrent;

namespace Arda9UserApi.Core.CQRS;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<Type, Type> _handlerCache = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = GetHandlerType(commandType, typeof(ICommandHandler<,>));
        
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for command {commandType.Name}");
        }

        var method = handlerType.GetMethod("HandleAsync");
        var task = (Task<TResponse>)method!.Invoke(handler, new object[] { command, cancellationToken })!;
        
        return await task;
    }

    public async Task<TResponse> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
    {
        var queryType = query.GetType();
        var handlerType = GetHandlerType(queryType, typeof(IQueryHandler<,>));
        
        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for query {queryType.Name}");
        }

        var method = handlerType.GetMethod("HandleAsync");
        var task = (Task<TResponse>)method!.Invoke(handler, new object[] { query, cancellationToken })!;
        
        return await task;
    }

    private static Type GetHandlerType(Type requestType, Type handlerInterfaceType)
    {
        return _handlerCache.GetOrAdd(requestType, _ =>
        {
            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && 
                           (i.GetGenericTypeDefinition() == typeof(ICommand<>) || 
                            i.GetGenericTypeDefinition() == typeof(IQuery<>)))
                .GetGenericArguments()[0];

            return handlerInterfaceType.MakeGenericType(requestType, responseType);
        });
    }
}