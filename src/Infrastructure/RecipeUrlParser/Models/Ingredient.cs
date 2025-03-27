namespace SharedCookbook.Infrastructure.RecipeUrlParser.Models;

// This only exists to deserialize based on matching property names from Spoonacular API
public abstract class Ingredient
{
    public string Name { get; set; } = "";
    public bool Optional { get; set; }
}
