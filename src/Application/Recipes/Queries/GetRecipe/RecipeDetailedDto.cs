﻿namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public class RecipeDetailedDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Author { get; set; }

    public string? AuthorEmail { get; set; }
    
    public string? Summary { get; init; }

    public string? Thumbnail { get; init; }

    public string? VideoPath { get; init; }

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

    public virtual ICollection<RecipeDirection> Directions { get; init; } = [];

    public virtual ICollection<RecipeImage> Images { get; init; } = [];

    public virtual ICollection<RecipeIngredient> Ingredients { get; init; } = [];
}
