﻿namespace SharedCookbook.Domain.Entities;

public class Recipe : BaseAuditableEntity
{
    public required int CookbookId { get; set; }

    public required string Title { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? VideoPath { get; set; }

    public int? PreparationTimeInMinutes { get; set; }

    public int? CookingTimeInMinutes { get; set; }

    public int? BakingTimeInMinutes { get; set; }

    public int? Servings { get; set; }

    public virtual Cookbook? Cookbook { get; set; }

    public virtual RecipeNutrition? Nutrition { get; set; }

    public virtual ICollection<CookbookNotification> CookbookNotifications { get; set; } = [];

    public virtual ICollection<IngredientCategory> IngredientCategories { get; set; } = [];

    public virtual ICollection<RecipeDirection> Directions { get; set; } = [];

    public virtual ICollection<RecipeIngredient> Ingredients { get; set; } = [];

    public virtual ICollection<RecipeImage> Images { get; set; } = [];
}
