namespace SharedCookbook.Application.Images.Commands.ParseRecipe;

public class RecipeFormInputs
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public int? PreparationTimeInMinutes { get; set; }
    public int? CookingTimeInMinutes { get; set; }
    public int? BakingTimeInMinutes { get; set; }
    public int? Servings { get; set; }
    public List<Ingredient> Ingredients { get; set; } = [];
    public List<Direction> Directions { get; set; } = [];
    public List<string> Images { get; set; } = [];

    public class Ingredient
    {
        public string Name { get; set; } = string.Empty;
        public bool? Optional { get; set; }
    }

    public class Direction
    {
        public string Text { get; set; } = string.Empty;
        public string? Image { get; set; }
    }
}
