namespace SharedCookbook.Domain.Entities;

public class Recipe : BaseAuditableEntity
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

    public Cookbook? Cookbook { get; set; }

    public RecipeNutrition? Nutrition { get; set; }

    public ICollection<CookbookNotification> CookbookNotifications { get; set; } = [];

    public ICollection<IngredientCategory> IngredientCategories { get; set; } = [];

    public ICollection<RecipeDirection> Directions { get; set; } = [];

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public ICollection<RecipeImage> Images { get; set; } = [];
}
