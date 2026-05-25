using SharedCookbook.Application.Common.Performance;

namespace SharedCookbook.Application.Common.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse>(
    ILogger<TRequest> logger,
    IUser user,
    IIdentityService identityService,
    TimeProvider timeProvider)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly long ThresholdMilliseconds = PerformanceThresholds.For(typeof(TRequest));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        long startTimestamp = timeProvider.GetTimestamp();

        var response = await next(cancellationToken);

        long elapsedMilliseconds = (long)timeProvider.GetElapsedTime(startTimestamp).TotalMilliseconds;

        if (elapsedMilliseconds <= ThresholdMilliseconds)
            return response;

        string requestName = typeof(TRequest).Name;
        string userId = user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
            userName = await identityService.GetUserNameAsync(userId);

        logger.LogWarning(
            "SharedCookbook Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds, threshold {ThresholdMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
            requestName, elapsedMilliseconds, ThresholdMilliseconds, userId, userName, request);

        return response;
    }
}
