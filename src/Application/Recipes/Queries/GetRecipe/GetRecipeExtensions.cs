using System.Linq.Expressions;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public static class GetRecipeDbQuery
{
    extension(IQueryable<Recipe> query)
    {
        public async Task<RecipeDetailedDto?> QueryDetailedDto(int id,
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

    extension(IEnumerable<RecipeDirection> directions)
    {
        private IEnumerable<RecipeDirection> Order() => directions.OrderBy(direction => direction.Ordinal);
        private List<RecipeDirectionDto> ToDtos() => directions.Select(ToDirectionDto).ToList();
    }

    private static readonly Func<RecipeDirection, RecipeDirectionDto> ToDirectionDto =
        direction => new RecipeDirectionDto
        {
            Id = direction.Id,
            Text = direction.Text,
            Ordinal = direction.Ordinal,
            Image = direction.Image
        };
    
    extension(IEnumerable<RecipeImage> images)
    {
        private IEnumerable<RecipeImage> Order() => images.OrderBy(direction => direction.Ordinal);

        private List<RecipeImageDto> ToDtos(string imageBaseUrl) =>
            images.Select(i => ToImageDto(i, imageBaseUrl)).ToList();
    }

    
private static readonly Func<RecipeImage, string, RecipeImageDto> ToImageDto =
    (image, imageBaseUrl) => new RecipeImageDto
    {
        Id = image.Id,
        Name = image.Name.PrefixIfNotEmpty(imageBaseUrl),
        Ordinal = image.Ordinal
    };
    
    extension(IEnumerable<RecipeIngredient> ingredients)
    {
        private IEnumerable<RecipeIngredient> Order() => ingredients.OrderBy(ingredient => ingredient.Ordinal);
        private List<RecipeIngredientDto> ToDtos() => ingredients.Select(ToIngredientDto).ToList();
    }

    private static readonly Func<RecipeIngredient, RecipeIngredientDto> ToIngredientDto =
        ingredient => new RecipeIngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Ordinal = ingredient.Ordinal,
            Optional = ingredient.Optional
        };
}
