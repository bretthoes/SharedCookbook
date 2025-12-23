using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class GetDetailedDtoByIdDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        internal async Task<RecipeDetailedDto?> GetDetailedDtoById(int id, string imageBaseUrl, CancellationToken ct)
            => await query.Where(recipe => recipe.Id == id)
                .Select(RecipeMapping.ToDetailedDto(imageBaseUrl))
                .SingleOrDefaultAsync(ct);
    }
}
