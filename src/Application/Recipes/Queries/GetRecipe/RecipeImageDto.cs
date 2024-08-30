using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeImageDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int Ordinal { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeImage, RecipeImageDto>();
        }
    }
}
