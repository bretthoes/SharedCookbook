namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This only exists to deserialize based on matching property names from Spoonacular API
public class Ingredient
{
    public string Original { get; set; } = "";

}
