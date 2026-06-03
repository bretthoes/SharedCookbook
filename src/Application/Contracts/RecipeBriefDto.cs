namespace SharedCookbook.Application.Contracts;

public sealed record RecipeBriefDto
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public bool? IsVegetarian { get; init; }
    public bool? IsVegan { get; init; }
    public bool? IsGlutenFree { get; init; }
    public bool? IsDairyFree { get; init; }
    public bool? IsHealthy { get; init; }
    public bool? IsCheap { get; init; }
    public bool? IsLowFodmap { get; init; }
    public bool? IsHighProtein { get; init; }
    public bool? IsBreakfast { get; init; }
    public bool? IsLunch { get; init; }
    public bool? IsDinner { get; init; }
    public bool? IsDessert { get; init; }
    public bool? IsSnack { get; init; }
}
