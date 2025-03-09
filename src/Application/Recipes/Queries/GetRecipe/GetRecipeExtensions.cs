namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

internal static class RecipeQueryExtensions
{
    public static async Task<RecipeDetailedDto?> FindRecipeDtoById(this DbSet<Recipe> db,
        int id,
        CancellationToken cancellationToken)
        => (await db.FindRecipe(id, cancellationToken))?
            .ProjectToDetailedDto();

    private static ValueTask<Recipe?> FindRecipe(this DbSet<Recipe> db,
        int id,
        CancellationToken cancellationToken)
        => db.FindAsync(keyValues: [id], cancellationToken);

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
