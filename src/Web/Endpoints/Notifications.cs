using SharedCookbook.Application.Notifications.Queries.GetLatestNotification;
using SharedCookbook.Application.Notifications.Queries.GetNotificationsWithPagination;

namespace SharedCookbook.Web.Endpoints;

public class Notifications : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(Latest, "latest")
            .RequireAuthorization()
            .Produces<NotificationDto>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapGet(List)
            .RequireAuthorization()
            .Produces<PaginatedList<NotificationDto>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> Latest(ISender sender, CancellationToken ct = default)
    {
        var notification = await sender.Send(new GetLatestNotificationQuery(), ct);
        return notification is null ? Results.NoContent() : Results.Ok(notification);
    }

    private static Task<PaginatedList<NotificationDto>> List(
        ISender sender,
        [AsParameters] GetNotificationsWithPaginationQuery query,
        CancellationToken ct = default) =>
        sender.Send(query, ct);
}
