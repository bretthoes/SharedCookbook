namespace SharedCookbook.Domain.ValueObjects;

public sealed class DietaryTags(
    bool? isVegetarian,
    bool? isVegan,
    bool? isGlutenFree,
    bool? isDairyFree,
    bool? isHealthy,
    bool? isCheap,
    bool? isLowFodmap,
    bool? isHighProtein) : ValueObject
{
    public bool? IsVegetarian { get; } = isVegetarian;
    public bool? IsVegan { get; } = isVegan;
    public bool? IsGlutenFree { get; } = isGlutenFree;
    public bool? IsDairyFree { get; } = isDairyFree;
    public bool? IsHealthy { get; } = isHealthy;
    public bool? IsCheap { get; } = isCheap;
    public bool? IsLowFodmap { get; } = isLowFodmap;
    public bool? IsHighProtein { get; } = isHighProtein;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return (IsVegetarian, IsVegan, IsGlutenFree, IsDairyFree, IsHealthy, IsCheap, IsLowFodmap, IsHighProtein);
    }
}
