namespace SharedCookbook.Application.Notifications.Queries.GetLatestNotification;

public sealed record GetLatestNotificationQuery : IRequest<NotificationDto?>;

public sealed class GetLatestNotificationQueryHandler(IIdentityRepository repository)
    : IRequestHandler<GetLatestNotificationQuery, NotificationDto?>
{
    public Task<NotificationDto?> Handle(GetLatestNotificationQuery request, CancellationToken ct = default) =>
        repository.GetLatestNotificationAsync(ct);
}
