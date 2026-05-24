using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Web.Infrastructure.RateLimiting;

namespace SharedCookbook.Web.Endpoints;

public class Subscriptions : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder builder)
    {
        builder.MapGet(GetStatus, pattern: "status").RequireAuthorization();
        builder.MapPost(RevenueCatWebhook, pattern: "webhook/revenuecat");
    }

    /// <summary>
    /// Returns the authenticated user's current subscription tier.
    /// The mobile app calls this on launch to verify server-side tier.
    /// </summary>
    private static IResult GetStatus(HttpContext httpContext)
    {
        var tier = httpContext.User.FindFirstValue(SubscriptionClaims.Tier)
                   ?? nameof(SubscriptionTier.Free);
        return Results.Ok(new SubscriptionStatusDto(tier == nameof(SubscriptionTier.Pro)));
    }

    /// <summary>
    /// Receives lifecycle events from RevenueCat and updates the user's
    /// SubscriptionTier so server-side rate limits reflect their entitlement.
    /// </summary>
    private static async Task<IResult> RevenueCatWebhook(
        HttpContext httpContext,
        UserManager<ApplicationUser> userManager,
        IOptions<RevenueCatWebhookOptions> webhookOptions,
        ILogger<Subscriptions> logger)
    {
        if (!ValidateSharedSecret(httpContext, webhookOptions.Value.SharedSecret))
        {
            logger.LogWarning("RevenueCat webhook rejected: invalid authorization header");
            return Results.Unauthorized();
        }

        RevenueCatWebhookPayload? payload;
        try
        {
            payload = await JsonSerializer.DeserializeAsync<RevenueCatWebhookPayload>(
                httpContext.Request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "RevenueCat webhook: failed to deserialize payload");
            return Results.BadRequest();
        }

        if (payload?.Event is null)
        {
            logger.LogWarning("RevenueCat webhook: missing event object");
            return Results.BadRequest();
        }

        var appUserId = payload.Event.AppUserId ?? payload.Event.OriginalAppUserId;
        if (string.IsNullOrWhiteSpace(appUserId))
        {
            logger.LogWarning("RevenueCat webhook: event has no app_user_id");
            return Results.Ok();
        }

        var user = await userManager.FindByIdAsync(appUserId);
        if (user is null)
        {
            logger.LogWarning("RevenueCat webhook: no user found for app_user_id {AppUserId}", appUserId);
            return Results.Ok();
        }

        var newTier = ResolveNewTier(payload.Event.Type);
        if (newTier is null)
        {
            logger.LogInformation(
                "RevenueCat webhook: event type {EventType} does not change tier for user {UserId}",
                payload.Event.Type, user.Id);
            return Results.Ok();
        }

        if (user.SubscriptionTier == newTier.Value)
            return Results.Ok();

        user.SubscriptionTier = newTier.Value;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            logger.LogError(
                "RevenueCat webhook: failed to update tier for user {UserId}: {Errors}",
                user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
            return Results.Problem("Failed to update subscription tier.");
        }

        logger.LogInformation(
            "RevenueCat webhook: user {UserId} tier set to {Tier} via {EventType}",
            user.Id, newTier.Value, payload.Event.Type);

        return Results.Ok();
    }

    private static bool ValidateSharedSecret(HttpContext httpContext, string configuredSecret)
    {
        if (string.IsNullOrWhiteSpace(configuredSecret))
            return true;

        var authHeader = httpContext.Request.Headers.Authorization.ToString();

        // RevenueCat sends the secret as a bare value in the Authorization header.
        return authHeader == configuredSecret;
    }

    /// <summary>
    /// Maps RevenueCat event types to a new SubscriptionTier.
    /// Returns null when the event does not warrant a tier change
    /// (e.g. CANCELLATION, which leaves the subscription active until EXPIRATION).
    /// </summary>
    private static SubscriptionTier? ResolveNewTier(string? eventType) => eventType switch
    {
        "INITIAL_PURCHASE" or
        "RENEWAL" or
        "UNCANCELLATION" or
        "PRODUCT_CHANGE" or
        "TRANSFER" or
        "TRIAL_CONVERTED" => SubscriptionTier.Pro,

        "EXPIRATION" => SubscriptionTier.Free,

        _ => null,
    };
}

internal sealed record SubscriptionStatusDto(bool IsPro);

internal sealed class RevenueCatWebhookPayload
{
    [JsonPropertyName("event")]
    public RevenueCatEvent? Event { get; set; }
}

internal sealed class RevenueCatEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("app_user_id")]
    public string? AppUserId { get; set; }

    [JsonPropertyName("original_app_user_id")]
    public string? OriginalAppUserId { get; set; }
}
