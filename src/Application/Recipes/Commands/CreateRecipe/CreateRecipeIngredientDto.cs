using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class CreateRecipeIngredientDto
{
    public required string Name { get; set; }

    public required bool Optional { get; set; }

    public required int Ordinal { get; set; }


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateRecipeIngredientDto, RecipeIngredient>();
        }
    }
}
