using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public class RecipeBriefDto
{
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, RecipeBriefDto>();
        }
    }
}
