namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class RecipeQueryExtensions
{
    public static async Task<RecipeDetailedDto?> QueryRecipeDetailedDto(this IQueryable<Recipe> query,
        int id,
        CancellationToken cancellationToken)
        => (await query
            .AsNoTracking()
            .IncludeRecipeDetails()
            .SingleAsync(recipe => recipe.Id == id, cancellationToken))
            .ProjectToDetailedDto();

    private static IQueryable<Recipe> IncludeRecipeDetails(this IQueryable<Recipe> query)
        => query
            .Include(recipe => recipe.Directions)
            .Include(recipe => recipe.Images)
            .Include(recipe => recipe.Ingredients);

    private static RecipeDetailedDto ProjectToDetailedDto(this Recipe recipe)
        => new()
        {
            Id = recipe.Id,
            Title = recipe.Title,
            AuthorId = recipe.CreatedBy,
            Summary = recipe.Summary,
            Thumbnail = recipe.Thumbnail,
            VideoPath = recipe.VideoPath,
            PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = recipe.BakingTimeInMinutes,
            Servings = recipe.Servings,
            Directions = recipe.Directions,
            Images = recipe.Images,
            Ingredients = recipe.Ingredients
        };
}
