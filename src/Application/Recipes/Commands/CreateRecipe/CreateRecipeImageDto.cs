using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeImageDto
{
    public required string Name { get; set; }

    public required int Ordinal { get; set; }
}
