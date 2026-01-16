using Microsoft.Extensions.DependencyInjection;
using DomainNotification = SharedCookbook.Domain.Common.INotification;

namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Default mediator implementation
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var isUnitResponse = responseType == typeof(Unit);

        // Get the handler - use single type arg interface for Unit responses
        object handler;
        Type handlerType;
        if (isUnitResponse)
        {
            handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            handler = serviceProvider.GetRequiredService(handlerType);
        }
        else
        {
            handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            handler = serviceProvider.GetRequiredService(handlerType);
        }

        // Get pre-processors
        var preProcessorType = typeof(IRequestPreProcessor<>).MakeGenericType(requestType);
        var preProcessors = serviceProvider.GetServices(preProcessorType);

        // Execute pre-processors
        foreach (var preProcessor in preProcessors)
        {
            var processMethod = preProcessorType.GetMethod("Process")!;
            var task = (Task)processMethod.Invoke(preProcessor, [request, cancellationToken])!;
            await task.ConfigureAwait(false);
        }

        // Get pipeline behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = serviceProvider.GetServices(behaviorType).Reverse().ToList();

        // Build the pipeline
        var handleMethod = handlerType.GetMethod("Handle")!;
        RequestHandlerDelegate<TResponse> handlerDelegate;
        
        if (isUnitResponse)
        {
            // For Unit responses, the handler returns Task (not Task<Unit>)
            handlerDelegate = async ct =>
            {
                var task = (Task)handleMethod.Invoke(handler, [request, ct])!;
                await task.ConfigureAwait(false);
                return (TResponse)(object)Unit.Value;
            };
        }
        else
        {
            handlerDelegate = async ct =>
            {
                var task = (Task<TResponse>)handleMethod.Invoke(handler, [request, ct])!;
                return await task.ConfigureAwait(false);
            };
        }

        // Wrap behaviors around the handler
        foreach (var behavior in behaviors)
        {
            var currentDelegate = handlerDelegate;
            var behaviorHandleMethod = behaviorType.GetMethod("Handle")!;
            handlerDelegate = async ct =>
            {
                var task = (Task<TResponse>)behaviorHandleMethod.Invoke(behavior, [request, currentDelegate, ct])!;
                return await task.ConfigureAwait(false);
            };
        }

        return await handlerDelegate(cancellationToken).ConfigureAwait(false);
    }

    public async Task<object?> Send(IBaseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        
        // Find the IRequest<TResponse> interface to get the response type
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        if (requestInterface == null)
            throw new InvalidOperationException($"Request type {requestType.Name} does not implement IRequest<TResponse>");

        var responseType = requestInterface.GetGenericArguments()[0];

        // Use reflection to call the generic Send method
        var sendMethod = typeof(Mediator)
            .GetMethods()
            .First(m => m.Name == "Send" && m.IsGenericMethod)
            .MakeGenericMethod(responseType);

        var task = (Task)sendMethod.Invoke(this, [request, cancellationToken])!;
        await task.ConfigureAwait(false);

        // Get the result from the task
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }

    public async Task Publish(DomainNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = serviceProvider.GetServices(handlerType);

        var handleMethod = handlerType.GetMethod("Handle")!;

        foreach (var handler in handlers)
        {
            var task = (Task)handleMethod.Invoke(handler, [notification, cancellationToken])!;
            await task.ConfigureAwait(false);
        }
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : DomainNotification
    {
        return Publish((DomainNotification)notification, cancellationToken);
    }
}
