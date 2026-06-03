namespace SharedCookbook.Domain.Entities;

public sealed class RecipeNutrition
{
    public int? Calories { get; init; }

    public int? Protein { get; init; }

    public int? Fat { get; init; }

    public int? Carbohydrates { get; init; }

    public int? Sugar { get; init; }

    public int? Fiber { get; init; }

    public int? Sodium { get; init; }
}
