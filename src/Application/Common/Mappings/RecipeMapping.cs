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
            PreparationTimeInMinutes = recipe.Timing.PreparationMinutes,
            CookingTimeInMinutes = recipe.Timing.CookingMinutes,
            BakingTimeInMinutes = recipe.Timing.BakingMinutes,
            Servings = recipe.Servings,
            Directions = recipe.Directions.ToDtos(imageBaseUrl).ToList(),
            Images = recipe.Images.ToDtos(imageBaseUrl).ToList(),
            IngredientSections = recipe.IngredientSections // TODO we can't abstract these LINQ methods due to nested collection; EF Core can't resolve the query otherwise 
                .OrderBy(section => section.Ordinal)
                .AsQueryable()
                .Select(IngredientSectionMapping.ToDtoExpression)
                .ToList(),
            IsVegetarian = recipe.DietaryTags.IsVegetarian,
            IsVegan = recipe.DietaryTags.IsVegan,
            IsGlutenFree = recipe.DietaryTags.IsGlutenFree,
            IsDairyFree = recipe.DietaryTags.IsDairyFree,
            IsHealthy = recipe.DietaryTags.IsHealthy,
            IsCheap = recipe.DietaryTags.IsCheap,
            IsLowFodmap = recipe.DietaryTags.IsLowFodmap,
            IsHighProtein = recipe.DietaryTags.IsHighProtein,
            IsBreakfast = recipe.MealTypes.IsBreakfast,
            IsLunch = recipe.MealTypes.IsLunch,
            IsDinner = recipe.MealTypes.IsDinner,
            IsDessert = recipe.MealTypes.IsDessert,
            IsSnack = recipe.MealTypes.IsSnack,
        };
}
