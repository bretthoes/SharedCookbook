using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDetailedDto
{
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, RecipeDetailedDto>();
        }
    }
}
