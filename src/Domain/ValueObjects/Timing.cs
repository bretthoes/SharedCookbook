namespace SharedCookbook.Domain.ValueObjects;

public sealed class Timing(
    int? preparationMinutes,
    int? cookingMinutes,
    int? bakingMinutes) : ValueObject
{
    public int? PreparationMinutes { get; } = preparationMinutes;
    public int? CookingMinutes { get; } = cookingMinutes;
    public int? BakingMinutes { get; } = bakingMinutes;

    public int TotalMinutes =>
        (PreparationMinutes ?? 0) + (CookingMinutes ?? 0) + (BakingMinutes ?? 0);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return (PreparationMinutes, CookingMinutes, BakingMinutes);
    }
}
