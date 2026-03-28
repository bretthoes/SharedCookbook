using System.Linq.Expressions;

namespace SharedCookbook.Application.Common.Mappings;

internal static class RecipeMapping
{
    internal static Expression<Func<Recipe, RecipeDetailedDto>> ToDetailedDto(string imageBaseUrl) =>
        recipe => new RecipeDetailedDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            PreparationTimeInMinutes = recipe.Timing != null ? recipe.Timing.PreparationMinutes : null,
            CookingTimeInMinutes = recipe.Timing != null ? recipe.Timing.CookingMinutes : null,
            BakingTimeInMinutes = recipe.Timing != null ? recipe.Timing.BakingMinutes : null,
            Servings = recipe.Servings,
            Directions = recipe.Directions.ToDtos(imageBaseUrl).ToList(),
            Images = recipe.Images.ToDtos(imageBaseUrl).ToList(),
            Ingredients = recipe.Ingredients.ToDtos().ToList(),
            IsVegan = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegan : null,
            IsVegetarian = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegetarian : null,
            IsCheap = recipe.DietaryTags != null ? recipe.DietaryTags.IsCheap : null,
            IsHealthy = recipe.DietaryTags != null ? recipe.DietaryTags.IsHealthy : null,
            IsDairyFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsDairyFree : null,
            IsGlutenFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsGlutenFree : null,
            IsLowFodmap = recipe.DietaryTags != null ? recipe.DietaryTags.IsLowFodmap : null,
        };
}
