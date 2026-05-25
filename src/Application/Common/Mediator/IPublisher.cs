using DomainNotification = SharedCookbook.Domain.Common.INotification;

namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Publish a notification to multiple handlers
/// </summary>
public interface IPublisher
{
    /// <summary>
    /// Asynchronously send a notification to multiple handlers
    /// </summary>
    /// <param name="notification">Notification object</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    Task Publish(DomainNotification notification, CancellationToken ct = default);

    /// <summary>
    /// Asynchronously send a notification to multiple handlers
    /// </summary>
    /// <typeparam name="TNotification">Notification type</typeparam>
    /// <param name="notification">Notification object</param>
    /// <param name="ct">Optional cancellation token</param>
    /// <returns>A task that represents the publish operation</returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken ct = default)
        where TNotification : DomainNotification;
}
