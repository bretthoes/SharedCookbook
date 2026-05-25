namespace SharedCookbook.Application.Common.Performance;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LongRunningRequestAttribute(LongRunningRequestCategory category) : Attribute
{
    public LongRunningRequestCategory Category { get; } = category;
}
