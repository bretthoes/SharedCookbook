namespace SharedCookbook.Domain.ValueObjects;

public sealed class MealTypes(
    bool? isBreakfast,
    bool? isLunch,
    bool? isDinner,
    bool? isDessert,
    bool? isSnack) : ValueObject
{
    public bool? IsBreakfast { get; } = isBreakfast;
    public bool? IsLunch { get; } = isLunch;
    public bool? IsDinner { get; } = isDinner;
    public bool? IsDessert { get; } = isDessert;
    public bool? IsSnack { get; } = isSnack;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return (IsBreakfast, IsLunch, IsDinner, IsDessert, IsSnack);
    }
}
