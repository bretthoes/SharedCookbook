using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.Projections;

internal static class RecipeProjections
{
    internal static IQueryable<RecipeDetailedDto> SelectRecipeDetailedDto(
        this IQueryable<Recipe> recipes,
        IQueryable<ApplicationUser> people,
        string imageBaseUrl) =>
        from recipe in recipes
        join applicationUser in people on recipe.CreatedBy equals applicationUser.Id
        select new RecipeDetailedDto
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Summary = recipe.Summary,
            AuthorEmail = applicationUser.Email ?? "",
            Author = applicationUser.DisplayName,
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
