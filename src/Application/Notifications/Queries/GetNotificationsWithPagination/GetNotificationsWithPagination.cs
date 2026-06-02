namespace SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

public sealed record GetNotificationsWithPaginationQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PaginatedList<NotificationDto>>;

public sealed class GetNotificationsWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetNotificationsWithPaginationQuery, PaginatedList<NotificationDto>>
{
    public Task<PaginatedList<NotificationDto>> Handle(
        GetNotificationsWithPaginationQuery request,
        CancellationToken ct = default)
        => context.CookbookNotifications
            .AsNoTracking()
            .QueryDtos(user.Id!, request.PageNumber, request.PageSize, ct);
}
