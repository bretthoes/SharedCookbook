using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.Entities;

public sealed class Recipe : BaseAuditableEntity
{
    public int CookbookId { get; set; }

    public required string Title { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? VideoPath { get; set; }

    public int? Servings { get; set; }

    public Timing? Timing { get; set; }

    public DietaryTags? DietaryTags { get; set; }

    public MealTypes? MealTypes { get; set; }

    public Cookbook? Cookbook { get; init; }

    public RecipeNutrition? Nutrition { get; init; }

    public ICollection<RecipeDirection> Directions { get; init; } = [];

    public ICollection<RecipeIngredient> Ingredients { get; init; } = [];

    public ICollection<RecipeImage> Images { get; init; } = [];

    public struct Constraints
    {
        public const int MaxImageLength = 6;
        public const int MaxDirectionCount = 40;
        public const int MaxIngredientCount = 40;
        public const int TitleMaxLength = 255;
        public const int SummaryMaxLength = 2048;
        public const int ThumbnailMaxLength = 2048;
        public const int VideoPathMaxLength = 2048;
        public const int MaxTimeInMinutes = 10080; // one week
    }
}
