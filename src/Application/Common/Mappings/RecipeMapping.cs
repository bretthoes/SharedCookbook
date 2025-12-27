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
            PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = recipe.BakingTimeInMinutes,
            Servings = recipe.Servings,
            Directions = recipe.Directions.ToDtos().ToList(),
            Images = recipe.Images.ToDtos(imageBaseUrl).ToList(),
            Ingredients = recipe.Ingredients.ToDtos().ToList(),
            IsVegan = recipe.IsVegan,
            IsVegetarian = recipe.IsVegetarian,
            IsCheap = recipe.IsCheap,
            IsHealthy = recipe.IsHealthy,
            IsDairyFree = recipe.IsDairyFree,
            IsGlutenFree = recipe.IsGlutenFree,
            IsLowFodmap = recipe.IsLowFodmap,
        };
}
