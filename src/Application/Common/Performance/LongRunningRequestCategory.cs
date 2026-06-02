namespace SharedCookbook.Application.Common.Performance;

public enum LongRunningRequestCategory
{
    ExternalApi = 0,
    FileProcessing = 1,
    DataPropagation = 2,
}
