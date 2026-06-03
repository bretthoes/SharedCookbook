namespace SharedCookbook.Application.Contracts;

public sealed record IngredientSectionDto
{
    public required string Title { get; init; }

    public required int Ordinal { get; init; }

    public required IReadOnlyList<RecipeIngredientDto> Ingredients { get; init; }

    public static IngredientSectionDto DefaultWrapper(IReadOnlyList<RecipeIngredientDto> ingredients) =>
        new() { Title = string.Empty, Ordinal = 0, Ingredients = ingredients };
}
