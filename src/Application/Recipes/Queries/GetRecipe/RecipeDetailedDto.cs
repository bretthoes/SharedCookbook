using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDetailedDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    public int AuthorId { get; set; }

    public string? Author { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? VideoPath { get; set; }

    public int? PreparationTimeInMinutes { get; set; }

    public int? CookingTimeInMinutes { get; set; }

    public int? BakingTimeInMinutes { get; set; }

    public int? Servings { get; set; }

    public virtual ICollection<RecipeDirectionDto> Directions { get; set; } = [];

    public virtual ICollection<RecipeImageDto> Images { get; set; } = [];

    public virtual ICollection<RecipeIngredientDto> Ingredients { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, RecipeDetailedDto>();
        }
    }
}
