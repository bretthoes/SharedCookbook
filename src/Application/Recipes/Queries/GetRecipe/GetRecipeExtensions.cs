using System.Linq.Expressions;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public static class RecipeQueryExtensions
{
    extension(IQueryable<Recipe> query)
    {
        public async Task<RecipeDetailedDto?> QueryDetailedDto(int id,
            string imageBaseUrl,
            CancellationToken cancellationToken)
            => await query.Select(ToDetailedDto(imageBaseUrl)).Where(dto => dto.Id == id).SingleOrDefaultAsync(cancellationToken);

        public IQueryable<Recipe> IncludeRecipeDetails()
            => query
                .Include(recipe => recipe.Directions)
                .Include(recipe => recipe.Images)
                .Include(recipe => recipe.Ingredients);
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
            Directions = recipe.Directions
                .OrderBy(d => d.Ordinal)
                .Select(d => new RecipeDirection { Id = d.Id, Ordinal = d.Ordinal, Text = d.Text })
                .ToList(),
            Images = recipe.Images
                .OrderBy(i => i.Ordinal)
                .Select(i => new RecipeImage { Id = i.Id, Name = imageBaseUrl + i.Name, Ordinal = i.Ordinal })
                .ToList(),
            Ingredients = recipe.Ingredients
                .OrderBy(i => i.Ordinal)
                .Select(i => new RecipeIngredient
                {
                    Id = i.Id, Ordinal = i.Ordinal, Name = i.Name, Optional = i.Optional
                })
                .ToList(),
            IsVegan = recipe.IsVegan,
            IsVegetarian = recipe.IsVegetarian,
            IsCheap = recipe.IsCheap,
            IsHealthy = recipe.IsHealthy,
            IsDairyFree = recipe.IsDairyFree,
            IsGlutenFree = recipe.IsGlutenFree,
            IsLowFodmap = recipe.IsLowFodmap,
        };
}
