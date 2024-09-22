using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class CreateRecipeDto
{

    public required string Title { get; set; }

    public int? AuthorId { get; set; }

    public string? Author { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? VideoPath { get; set; }

    public int? PreparationTimeInMinutes { get; set; }

    public int? CookingTimeInMinutes { get; set; }

    public int? BakingTimeInMinutes { get; set; }

    public int? Servings { get; set; }

    public virtual ICollection<CreateRecipeDirectionDto> Directions { get; set; } = [];

    public virtual ICollection<CreateRecipeImageDto> Images { get; set; } = [];

    public virtual ICollection<CreateRecipeIngredientDto> Ingredients { get; set; } = [];

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Recipe, CreateRecipeDto>();
        }
    }
}
