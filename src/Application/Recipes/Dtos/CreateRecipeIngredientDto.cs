namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeIngredientDto
{
    public required string Name { get; init; }

    public required bool Optional { get; init; }

    public required int Ordinal { get; init; }
    
    public RecipeIngredient ToEntity() => new() { Name = Name, Optional = Optional, Ordinal = Ordinal };
}

public static partial class Extensions
{
    public static IEnumerable<RecipeIngredient> ToEntities(
        this IEnumerable<CreateRecipeIngredientDto> recipeIngredients)
        => recipeIngredients.Select(ingredient => ingredient.ToEntity());
}
