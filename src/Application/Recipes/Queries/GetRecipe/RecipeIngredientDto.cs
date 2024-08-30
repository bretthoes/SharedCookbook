using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeIngredientDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int Ordinal { get; set; }

    public required bool Optional { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeIngredient, RecipeIngredientDto>();
        }
    }
}
