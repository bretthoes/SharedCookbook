using SharedCookbook.Application.Recipes.Queries.GetRecipe;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeDto
{
    public required string Title { get; set; }
    public required int CookbookId { get; set; }
    public string? Summary { get; set; }
    public string? Thumbnail { get; set; }
    public string? VideoPath { get; set; }
    public int? PreparationTimeInMinutes { get; set; }
    public int? CookingTimeInMinutes { get; set; }
    public int? BakingTimeInMinutes { get; set; }
    public int? Servings { get; set; }
    public ICollection<CreateRecipeDirectionDto> Directions { get; set; } = [];
    public ICollection<CreateRecipeImageDto> Images { get; set; } = [];
    public ICollection<CreateRecipeIngredientDto> Ingredients { get; set; } = [];
}
