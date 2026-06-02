using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

namespace SharedCookbook.Application.Notifications.Queries.GetLatestNotification;

public sealed record GetLatestNotificationQuery : IRequest<NotificationDto?>;

public sealed class GetLatestNotificationQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetLatestNotificationQuery, NotificationDto?>
{
    public Task<NotificationDto?> Handle(GetLatestNotificationQuery request, CancellationToken ct = default)
        => context.CookbookNotifications
            .AsNoTracking()
            .QueryLatest(user.Id!, ct);
}
