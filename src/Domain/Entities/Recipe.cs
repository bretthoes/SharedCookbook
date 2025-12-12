namespace SharedCookbook.Domain.Entities;

public sealed class Recipe : BaseAuditableEntity
{
    public int CookbookId { get; set; }

    public required string Title { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? VideoPath { get; set; }

    public int? PreparationTimeInMinutes { get; set; }

    public int? CookingTimeInMinutes { get; set; }

    public int? BakingTimeInMinutes { get; set; }

    public int? Servings { get; set; }
    
    public bool? IsVegetarian { get; init; }
    
    public bool? IsVegan { get; init; }
    
    public bool? IsGlutenFree { get; init; }
    
    public bool? IsDairyFree { get; init; }
    
    public bool? IsHealthy { get; init; }
    
    public bool? IsCheap { get; init; }
    
    public bool? IsLowFodmap { get; init; }
    
    public Cookbook? Cookbook { get; init; }

    public RecipeNutrition? Nutrition { get; init; }

    public IReadOnlyCollection<CookbookNotification> CookbookNotifications { get; init; } = [];

    public ICollection<IngredientCategory> IngredientCategories { get; init; } = [];

    public ICollection<RecipeDirection> Directions { get; init; } = [];

    public ICollection<RecipeIngredient> Ingredients { get; init; } = [];

    public ICollection<RecipeImage> Images { get; init; } = [];

    public struct Constraints
    {
        public const int TitleMaxLength = 255;
        public const int SummaryMaxLength = 2048;
        public const int ThumbnailMaxLength = 2048;
        public const int VideoPathMaxLength = 2048;
    }
}
