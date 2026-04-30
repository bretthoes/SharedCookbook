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
            IngredientSections = recipe.IngredientSections // TODO we can't abstract these LINQ methods due to nested collection; EF Core can't resolve the query otherwise 
                .OrderBy(section => section.Ordinal)
                .AsQueryable()
                .Select(IngredientSectionMapping.ToDtoExpression)
                .ToList(),
            IsVegetarian = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegetarian : null,
            IsVegan = recipe.DietaryTags != null ? recipe.DietaryTags.IsVegan : null,
            IsGlutenFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsGlutenFree : null,
            IsDairyFree = recipe.DietaryTags != null ? recipe.DietaryTags.IsDairyFree : null,
            IsHealthy = recipe.DietaryTags != null ? recipe.DietaryTags.IsHealthy : null,
            IsCheap = recipe.DietaryTags != null ? recipe.DietaryTags.IsCheap : null,
            IsLowFodmap = recipe.DietaryTags != null ? recipe.DietaryTags.IsLowFodmap : null,
            IsHighProtein = recipe.DietaryTags != null ? recipe.DietaryTags.IsHighProtein : null,
            IsBreakfast = recipe.MealTypes != null ? recipe.MealTypes.IsBreakfast : null,
            IsLunch = recipe.MealTypes != null ? recipe.MealTypes.IsLunch : null,
            IsDinner = recipe.MealTypes != null ? recipe.MealTypes.IsDinner : null,
            IsDessert = recipe.MealTypes != null ? recipe.MealTypes.IsDessert : null,
            IsSnack = recipe.MealTypes != null ? recipe.MealTypes.IsSnack : null,
        };
}
