namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This only exists to deserialize based on matching property names from Spoonacular API
public class RecipeApiResponse
{
    public string Title { get; init; } = "";
    public string? Summary { get; init; }
    public string? Image { get; init; }
    public int? Servings { get; init; }
    public List<Ingredient> ExtendedIngredients { get; init; } = [];
    public string Instructions { get; init; } = "";
}
