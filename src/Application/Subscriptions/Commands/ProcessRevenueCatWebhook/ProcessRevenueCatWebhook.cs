using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Security;
using SharedCookbook.Application.Subscriptions;

namespace SharedCookbook.Application.Subscriptions.Commands.ProcessRevenueCatWebhook;

[AllowAnonymous]
public sealed record ProcessRevenueCatWebhookCommand(
    string AuthorizationHeader,
    RevenueCatWebhookPayload? Payload)
    : IRequest<ProcessRevenueCatWebhookResult>;

public enum ProcessRevenueCatWebhookStatus
{
    Ok,
    Unauthorized,
    BadRequest,
    Failed,
}

public sealed record ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus Status);

public sealed class ProcessRevenueCatWebhookCommandHandler(
    IOptions<RevenueCatWebhookOptions> webhookOptions,
    IIdentityService identityService,
    ILogger<ProcessRevenueCatWebhookCommandHandler> logger)
    : IRequestHandler<ProcessRevenueCatWebhookCommand, ProcessRevenueCatWebhookResult>
{
    public async Task<ProcessRevenueCatWebhookResult> Handle(
        ProcessRevenueCatWebhookCommand request,
        CancellationToken ct = default)
    {
        if (!ValidateSharedSecret(request.AuthorizationHeader, webhookOptions.Value.SharedSecret))
        {
            logger.LogWarning("RevenueCat webhook rejected: invalid authorization header");
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Unauthorized);
        }

        if (request.Payload?.Event is null)
        {
            logger.LogWarning("RevenueCat webhook: missing event object");
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.BadRequest);
        }

        var appUserId = request.Payload.Event.AppUserId ?? request.Payload.Event.OriginalAppUserId;
        if (string.IsNullOrWhiteSpace(appUserId))
        {
            logger.LogWarning("RevenueCat webhook: event has no app_user_id");
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Ok);
        }

        var newTier = ResolveNewTier(request.Payload.Event.Type);
        if (newTier is null)
        {
            logger.LogInformation(
                "RevenueCat webhook: event type {EventType} does not change tier for app_user_id {AppUserId}",
                request.Payload.Event.Type, appUserId);
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Ok);
        }

        var result = await identityService.SetSubscriptionTierIfChangedAsync(appUserId, newTier, ct);
        if (result.IsUserNotFound)
        {
            logger.LogWarning("RevenueCat webhook: no user found for app_user_id {AppUserId}", appUserId);
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Ok);
        }

        if (!result.Result.Succeeded)
        {
            logger.LogError(
                "RevenueCat webhook: failed to update tier for app_user_id {AppUserId}: {Errors}",
                appUserId, string.Join(", ", result.Result.Errors));
            return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Failed);
        }

        if (result.WasUpdated)
        {
            logger.LogInformation(
                "RevenueCat webhook: user {AppUserId} tier set to {Tier} via {EventType}",
                appUserId, newTier, request.Payload.Event.Type);
        }

        return new ProcessRevenueCatWebhookResult(ProcessRevenueCatWebhookStatus.Ok);
    }

    private static bool ValidateSharedSecret(string authorizationHeader, string configuredSecret)
    {
        if (string.IsNullOrWhiteSpace(configuredSecret))
            return true;

        // RevenueCat sends the secret as a bare value in the Authorization header.
        return authorizationHeader == configuredSecret;
    }

    /// <summary>
    /// Maps RevenueCat event types to a new subscription tier name.
    /// Returns null when the event does not warrant a tier change
    /// (e.g. CANCELLATION, which leaves the subscription active until EXPIRATION).
    /// </summary>
    private static string? ResolveNewTier(string? eventType) => eventType switch
    {
        "INITIAL_PURCHASE" or
        "RENEWAL" or
        "UNCANCELLATION" or
        "PRODUCT_CHANGE" or
        "TRANSFER" or
        "TRIAL_CONVERTED" => "Pro",

        "EXPIRATION" => "Free",

        _ => null,
    };
}

public sealed class RevenueCatWebhookPayload
{
    [JsonPropertyName("event")]
    public RevenueCatEvent? Event { get; set; }
}

public sealed class RevenueCatEvent
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("app_user_id")]
    public string? AppUserId { get; set; }

    [JsonPropertyName("original_app_user_id")]
    public string? OriginalAppUserId { get; set; }
}
