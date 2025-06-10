namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This class only exists to deserialize expected response based on
// matching property names from Spoonacular. See below for more info: 
// https://spoonacular.com/food-api/docs#Extract-Recipe-from-Website
public class RecipeApiResponse
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string? Image { get; set; }
    public int? Servings { get; set; }
    public List<Ingredient>? ExtendedIngredients { get; set; }
    public string? Instructions { get; set; }
    public int? PreparationMinutes { get; set; }
    public int? CookingMinutes { get; set; }
}

public static class RecipeApiResponseExtensions
{
    public static RecipeApiResponse ApplyDefaults(this RecipeApiResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        response.Title ??= string.Empty;
        response.Summary ??= string.Empty;
        response.Image ??= string.Empty;
        response.Instructions ??= string.Empty;

        response.Servings ??= 0;
        response.PreparationMinutes ??= 0;
        response.CookingMinutes ??= 0;

        response.ExtendedIngredients ??= [];

        return response;
    }
}
