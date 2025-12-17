using System.Linq.Expressions;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public static class RecipeQueryExtensions
{
    extension(IQueryable<Recipe> query)
    {
        public async Task<RecipeDetailedDto?> QueryDetailedDto(int id,
            string imageBaseUrl,
            CancellationToken cancellationToken)
            => (await query
                    .AsNoTracking()
                    .IncludeRecipeDetails()
                    .SingleOrDefaultAsync(recipe => recipe.Id == id, cancellationToken))?
                .ToDetailedDto(imageBaseUrl);

        public IQueryable<Recipe> IncludeRecipeDetails()
            => query
                .Include(recipe => recipe.Directions)
                .Include(recipe => recipe.Images)
                .Include(recipe => recipe.Ingredients);
    }
    

    private static RecipeDetailedDto ToDetailedDto(this Recipe recipe, string imageBaseUrl)
        => new()
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            Thumbnail = string.IsNullOrEmpty(recipe.Thumbnail) ? "" : imageBaseUrl + recipe.Thumbnail,
            VideoPath = string.IsNullOrEmpty(recipe.VideoPath) ? "" : imageBaseUrl + recipe.VideoPath,
            PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = recipe.BakingTimeInMinutes,
            Servings = recipe.Servings,
            Directions = recipe.Directions,
            Images = recipe.Images
                .OrderBy(image => image.Ordinal)
                .Select(image => new RecipeImage
                {
                    Id = image.Id,
                    Name = imageBaseUrl + image.Name,
                    Ordinal = image.Ordinal,
                })
                .ToList(),
            Ingredients = recipe.Ingredients,
            IsVegan = recipe.IsVegan,
            IsVegetarian = recipe.IsVegetarian,
            IsCheap = recipe.IsCheap,
            IsHealthy = recipe.IsHealthy,
            IsDairyFree = recipe.IsDairyFree,
            IsGlutenFree = recipe.IsGlutenFree,
            IsLowFodmap = recipe.IsLowFodmap,
        };
}
