namespace SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

public sealed record GetNotificationsWithPaginationQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PaginatedList<NotificationDto>>;

public sealed class GetNotificationsWithPaginationQueryHandler(IIdentityRepository repository)
    : IRequestHandler<GetNotificationsWithPaginationQuery, PaginatedList<NotificationDto>>
{
    public Task<PaginatedList<NotificationDto>> Handle(
        GetNotificationsWithPaginationQuery request,
        CancellationToken ct = default) =>
        repository.GetNotifications(request, ct);
}
