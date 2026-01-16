namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Defines a pre-processor for a request
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
public interface IRequestPreProcessor<in TRequest>
    where TRequest : notnull
{
    /// <summary>
    /// Process method executes before calling the Handle method on your handler
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task Process(TRequest request, CancellationToken cancellationToken);
}
