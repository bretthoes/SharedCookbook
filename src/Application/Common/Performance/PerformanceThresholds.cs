namespace SharedCookbook.Application.Common.Performance;

internal static class PerformanceThresholds
{
    public const long DefaultMilliseconds = 500;
    public const long ExternalApiMilliseconds = 5_000;
    public const long FileProcessingMilliseconds = 3_000;
    public const long DataPropagationMilliseconds = 3_000;

    public static long For(LongRunningRequestCategory category) => category switch
    {
        LongRunningRequestCategory.ExternalApi => ExternalApiMilliseconds,
        LongRunningRequestCategory.FileProcessing => FileProcessingMilliseconds,
        LongRunningRequestCategory.DataPropagation => DataPropagationMilliseconds,
        _ => DefaultMilliseconds
    };

    public static long For(Type requestType)
    {
        LongRunningRequestAttribute? attribute =
            requestType.GetCustomAttribute<LongRunningRequestAttribute>();

        return attribute is null
            ? DefaultMilliseconds
            : For(attribute.Category);
    }
}
