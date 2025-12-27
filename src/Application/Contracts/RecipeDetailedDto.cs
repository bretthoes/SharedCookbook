namespace SharedCookbook.Application.Contracts;

public sealed class RecipeDetailedDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Author { get; set; }

    public string? AuthorEmail { get; set; }
    
    public string? Summary { get; init; }

    public string? Thumbnail { get; }

    public string? VideoPath { get; }

    public int? PreparationTimeInMinutes { get; init; }

    public int? CookingTimeInMinutes { get; init; }

    public int? BakingTimeInMinutes { get; init; }

    public int? Servings { get; init; }
    
    public bool? IsVegetarian { get; set; }
    
    public bool? IsVegan { get; set; }
    
    public bool? IsGlutenFree { get; set; }
    
    public bool? IsDairyFree { get; set; }
    
    public bool? IsHealthy { get; set; }
    
    public bool? IsCheap { get; set; }
    
    public bool? IsLowFodmap { get; set; }

    public IReadOnlyCollection<RecipeDirectionDto> Directions { get; init; } = [];

    public IReadOnlyCollection<RecipeImageDto> Images { get; init; } = [];

    public IReadOnlyCollection<RecipeIngredientDto> Ingredients { get; init; } = [];
}
