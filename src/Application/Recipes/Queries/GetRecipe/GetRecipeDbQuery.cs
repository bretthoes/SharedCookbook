using System.Linq.Expressions;
using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class GetRecipeDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal async Task<RecipeDetailedDto?> QueryDetailedDto(int id,
            string imageBaseUrl,
            CancellationToken cancellationToken) =>
            await query.Select(ToDetailedDto(imageBaseUrl))
                .Where(dto => dto.Id == id)
                .SingleOrDefaultAsync(cancellationToken);
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
            Directions = recipe.Directions.Order().ToDtos(),
            Images = recipe.Images.Order().ToDtos(imageBaseUrl),
            Ingredients = recipe.Ingredients.Order().ToDtos(),
            IsVegan = recipe.IsVegan,
            IsVegetarian = recipe.IsVegetarian,
            IsCheap = recipe.IsCheap,
            IsHealthy = recipe.IsHealthy,
            IsDairyFree = recipe.IsDairyFree,
            IsGlutenFree = recipe.IsGlutenFree,
            IsLowFodmap = recipe.IsLowFodmap,
        };
}
