using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public sealed class UpdateRecipeDto : CreateRecipeDto
{
    public int Id { get; init; }
}
