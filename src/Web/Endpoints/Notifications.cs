using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Notifications : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(List)
            .RequireAuthorization()
            .Produces<PaginatedList<NotificationDto>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static Task<PaginatedList<NotificationDto>> List(
        ISender sender,
        [AsParameters] GetNotificationsWithPaginationQuery query,
        CancellationToken ct = default) =>
        sender.Send(query, ct);
}
