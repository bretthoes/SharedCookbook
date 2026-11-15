using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace SharedCookbook.Web.Infrastructure.RateLimiting;

/// <summary>
/// Applied to the whole Identity API group, but only limits the endpoints that send email.
/// Login, refresh, and the rest pass through to the global limiter.
/// </summary>
internal sealed class AccountEmailRateLimiterPolicy(
    ILogger<AccountEmailRateLimiterPolicy> logger,
    IOptions<AccountEmailRateLimitOptions> options)
    : IRateLimiterPolicy<string>
{
    private static readonly string[] EmailSendingPathSuffixes =
    [
        "/register",
        "/resendConfirmationEmail",
        "/forgotPassword",
    ];

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        string path = httpContext.Request.Path.Value ?? string.Empty;
        if (!EmailSendingPathSuffixes.Any(suffix => path.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)))
            return RateLimitPartition.GetNoLimiter(string.Empty);

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: $"account-email:{RateLimitPartitionKeys.GetClientKey(httpContext)}",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = options.Value.PermitLimit,
                Window = TimeSpan.FromMinutes(options.Value.WindowMinutes),
            });
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected => async (context, ct) =>
    {
        logger.LogWarning(
            "Client {ClientKey} reached the account email rate limit on {Path}",
            RateLimitPartitionKeys.GetClientKey(context.HttpContext),
            context.HttpContext.Request.Path);

        await RateLimiterRejectionHandler.RejectAsync(context, ct);
    };
}
