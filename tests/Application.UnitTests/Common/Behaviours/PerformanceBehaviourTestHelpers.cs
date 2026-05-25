using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Mediator;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

internal static class PerformanceBehaviourTestHelpers
{
    public static TimeProvider CreateTimeProvider(long elapsedMilliseconds) =>
        new ControllableTimeProvider(elapsedMilliseconds);

    public static RequestHandlerDelegate<TResponse> ImmediateHandler<TResponse>(TResponse response) =>
        cancellationToken => Task.FromResult(response);

    public static void VerifyLogWarning<T>(Mock<ILogger<T>> logger, Times times) =>
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);

    private sealed class ControllableTimeProvider : TimeProvider
    {
        private readonly long _startTimestamp = 1_000;
        private readonly long _endTimestamp;
        private int _getTimestampCallCount;

        public ControllableTimeProvider(long elapsedMilliseconds)
        {
            _endTimestamp = _startTimestamp + (long)(elapsedMilliseconds * TimestampFrequency / 1_000.0);
        }

        public override long GetTimestamp() =>
            ++_getTimestampCallCount == 1 ? _startTimestamp : _endTimestamp;
    }
}
