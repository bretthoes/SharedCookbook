using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeIngredientDto
{
    public required string Name { get; init; }

    public required bool Optional { get; init; }

    public required int Ordinal { get; init; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateRecipeIngredientDto, RecipeIngredient>();
        }
    }
}
