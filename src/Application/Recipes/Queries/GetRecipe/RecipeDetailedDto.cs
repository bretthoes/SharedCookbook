using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDetailedDto
{
    public required int CookbookId { get; set; }

    public required string Title { get; set; }

    public required int AuthorId { get; set; }

    public string? Author { get; set; }

    public string? Summary { get; set; }

    public string? ImagePath { get; set; }

    public string? VideoPath { get; set; }

    public int? PreparationTimeInMinutes { get; set; }

    public int? CookingTimeInMinutes { get; set; }

    public int? BakingTimeInMinutes { get; set; }

    public int? Servings { get; set; }

    public virtual ICollection<RecipeDirection> Directions { get; set; } = [];

    public virtual ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, RecipeDetailedDto>();
        }
    }
}
