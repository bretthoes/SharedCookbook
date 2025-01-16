using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDirectionDto
{
    public required int Id { get; init; }

    public required string Text { get; init; }

    public required int Ordinal { get; init; }

    public string? Image { get; init; }
}
