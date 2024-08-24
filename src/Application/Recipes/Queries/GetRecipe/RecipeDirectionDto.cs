using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDirectionDto
{
    public required int Id { get; set; }

    public required string DirectionText { get; set; }

    public required int Ordinal { get; set; }

    public string? ImagePath { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<RecipeDirection, RecipeDirectionDto>();
        }
    }
}
