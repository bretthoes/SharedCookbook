using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Tests.Shared;

public static class RecipeTestData
{
    public const string Title = "Delicious Pasta";
    public const string Summary = "A wonderful pasta dish";
    public const int Servings = 4;
    public const int PreparationTimeInMinutes = 10;
    public const int CookingTimeInMinutes = 20;
    public const int BakingTimeInMinutes = 5;
    
    public const string IngredientName = "Spaghetti";
    public const int IngredientOrdinal = 1;
    public const bool IngredientOptional = false;
    
    public const string DirectionText = "Boil water and cook pasta";
    public const int DirectionOrdinal = 1;

    public const string ImageName = "pasta-image.jpg";
    public const int ImageOrdinal = 1;
    
    public static CreateRecipeDto GetSimpleCreateRecipeDto(int cookbookId) => new()
    {
        Title = Title,
        CookbookId = cookbookId,
        Summary = Summary,
        PreparationTimeInMinutes = PreparationTimeInMinutes,
        CookingTimeInMinutes = CookingTimeInMinutes,
        BakingTimeInMinutes = BakingTimeInMinutes,
        Servings = Servings,
        Ingredients =
        [
            new RecipeIngredientDto
            {
                Name = IngredientName, Ordinal = IngredientOrdinal, Optional = IngredientOptional
            }
        ],
        Directions =
        [
            new RecipeDirectionDto { Text = DirectionText, Ordinal = DirectionOrdinal, Image = null }
        ],
        Images =
        [
            new RecipeImageDto { Name = ImageName, Ordinal = ImageOrdinal }
        ]
    };
    
    public static UpdateRecipeDto GetSimpleUpdateRecipeDto(int id, string title = Title) => new()
    {
        Id = id,
        Title = title,
        Summary = Summary,
        PreparationTimeInMinutes = PreparationTimeInMinutes,
        CookingTimeInMinutes = CookingTimeInMinutes,
        BakingTimeInMinutes = BakingTimeInMinutes,
        Servings = Servings,
        Ingredients =
        [
            new RecipeIngredientDto
            {
                Name = IngredientName, Ordinal = IngredientOrdinal, Optional = IngredientOptional
            }
        ],
        Directions =
        [
            new RecipeDirectionDto { Text = DirectionText, Ordinal = DirectionOrdinal, Image = null }
        ],
        Images =
        [
            new RecipeImageDto { Name = ImageName, Ordinal = ImageOrdinal }
        ]
    };
}
