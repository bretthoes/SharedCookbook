using System.Linq.Expressions;
using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class GetByIdDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal async Task<RecipeDetailedDto?> QueryDetailedDto(int id, string imageBaseUrl, CancellationToken ct) =>
            await query.Where(recipe => recipe.Id == id)
                .Select(ToDetailedDto(imageBaseUrl))
                .SingleOrDefaultAsync(ct);
    }

    private static Expression<Func<Recipe, RecipeDetailedDto>> ToDetailedDto(string imageBaseUrl) =>
        recipe => new RecipeDetailedDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            Thumbnail = recipe.Thumbnail.PrefixIfNotEmpty(imageBaseUrl),
            VideoPath = recipe.VideoPath.PrefixIfNotEmpty(imageBaseUrl),
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
