using SharedCookbook.Application.Subscriptions.Commands.ProcessRevenueCatWebhook;
using SharedCookbook.Application.Subscriptions.Queries.GetSubscriptionStatus;

namespace SharedCookbook.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetStatus, pattern: "status").RequireAuthorization();
        builder.MapPost(RevenueCatWebhook, pattern: "webhook/revenuecat");
    }

    private static Task<SubscriptionStatusDto> GetStatus(ISender sender) =>
        sender.Send(new GetSubscriptionStatusQuery());

    private static async Task<IResult> RevenueCatWebhook(
        ISender sender,
        HttpContext httpContext,
        [FromBody] RevenueCatWebhookPayload? payload)
    {
        var result = await sender.Send(new ProcessRevenueCatWebhookCommand(
            httpContext.Request.Headers.Authorization.ToString(),
            payload));

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
