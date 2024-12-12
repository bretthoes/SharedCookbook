using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeImageDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeImage, RecipeImageDto>();
        }
    }
}
