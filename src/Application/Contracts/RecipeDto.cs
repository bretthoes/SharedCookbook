namespace SharedCookbook.Application.Contracts;

public abstract record RecipeDto
{
    public required string Title { get; init; }
    public string? Summary { get; init; }
    public string? Thumbnail { get; init; }
    public string? VideoPath { get; init; }
    public int? PreparationTimeInMinutes { get; init; }
    public int? CookingTimeInMinutes { get; init; }
    public int? BakingTimeInMinutes { get; init; }
    public int? Servings { get; init; }
    public ICollection<RecipeDirectionDto> Directions { get; init; } = [];
    public ICollection<RecipeImageDto> Images { get; init; } = [];
    public ICollection<RecipeIngredientDto> Ingredients { get; init; } = [];
}
