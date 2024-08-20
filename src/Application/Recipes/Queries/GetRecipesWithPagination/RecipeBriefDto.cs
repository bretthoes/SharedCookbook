using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public class RecipeBriefDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, RecipeBriefDto>();
        }
    }
}
