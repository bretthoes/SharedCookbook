using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class GetDetailedDtoByIdDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal async Task<RecipeDetailedDto?> GetDetailedDtoById(
            int id,
            string imageBaseUrl,
            CancellationToken ct = default)
            => await query.Where(recipe => recipe.Id == id)
                .AsSplitQuery() // TODO verify this improves performance; a recipe can only have so many directions, images, ingredients, etc. Find max, avg, and suppress warning if the extra round trips slow down query
                .Select(RecipeMapping.ToDetailedDto(imageBaseUrl))
                .SingleOrDefaultAsync(ct);
    }
}
