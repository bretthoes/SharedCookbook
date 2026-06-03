using System.Text.Json.Serialization;

namespace SharedCookbook.Application.Contracts;

public sealed class RecipeDetailedDto
{
    [JsonIgnore]
    internal string? AuthorId { get; init; }
    
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public string? Author { get; init; }

    public bool IsAuthor { get; set; }

    public string? Summary { get; init; }

    public string? Thumbnail { get; }

    public string? VideoPath { get; }

    public int? PreparationTimeInMinutes { get; init; }

    public int? CookingTimeInMinutes { get; init; }

    public int? BakingTimeInMinutes { get; init; }

    public int? Servings { get; init; }

    public int MadeCount { get; init; }
    
    public bool? IsVegetarian { get; init; }
    public bool? IsVegan { get; init; }
    public bool? IsGlutenFree { get; init; }
    public bool? IsDairyFree { get; init; }
    public bool? IsHealthy { get; init; }
    public bool? IsCheap { get; init; }
    public bool? IsLowFodmap { get; init; }
    public bool? IsHighProtein { get; init; }
    public bool? IsBreakfast { get; init; }
    public bool? IsLunch { get; init; }
    public bool? IsDinner { get; init; }
    public bool? IsDessert { get; init; }
    public bool? IsSnack { get; init; }

    public IReadOnlyCollection<RecipeDirectionDto> Directions { get; init; } = [];

    public IReadOnlyCollection<RecipeImageDto> Images { get; init; } = [];

    public IReadOnlyCollection<IngredientSectionDto> IngredientSections { get; init; } = [];
}
