using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeDirectionDto
{
    public required string Text { get; init; }

    public required int Ordinal { get; init; }

    public string? Image { get; init; }
}
