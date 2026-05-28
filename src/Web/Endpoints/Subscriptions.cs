using SharedCookbook.Application.Subscriptions.Commands.ProcessRevenueCatWebhook;
using SharedCookbook.Application.Subscriptions.Queries.GetSubscriptionStatus;

namespace SharedCookbook.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetStatus, pattern: "status")
            .RequireAuthorization()
            .Produces<SubscriptionStatusDto>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        builder.MapPost(RevenueCatWebhook, pattern: "webhook/revenuecat")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static Task<SubscriptionStatusDto> GetStatus(ISender sender, CancellationToken ct = default) =>
        sender.Send(new GetSubscriptionStatusQuery(), ct);

    private static async Task<IResult> RevenueCatWebhook(
        ISender sender,
        HttpContext httpContext,
        [FromBody] RevenueCatWebhookPayload? payload,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ProcessRevenueCatWebhookCommand(
            httpContext.Request.Headers.Authorization.ToString(),
            payload), ct);

        return result.Status switch
        {
            ProcessRevenueCatWebhookStatus.Ok => Results.Ok(),
            ProcessRevenueCatWebhookStatus.Unauthorized => Results.Unauthorized(),
            ProcessRevenueCatWebhookStatus.BadRequest => Results.BadRequest(),
            ProcessRevenueCatWebhookStatus.Failed => Results.Problem("Failed to update subscription tier."),
            _ => Results.Ok(),
        };
    }
}
