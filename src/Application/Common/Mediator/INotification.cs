using DomainNotification = SharedCookbook.Domain.Common.INotification;

namespace SharedCookbook.Application.Common.Mediator;

/// <summary>
/// Marker interface for notifications (events) - re-exports the Domain interface
/// </summary>
public interface INotification : DomainNotification;
