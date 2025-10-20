namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeDirectionDto
{
    public required string Text { get; init; }

    public required int Ordinal { get; init; }

    public string? Image { get; init; }

    public RecipeDirection ToEntity() => new() { Text = Text, Image = Image, Ordinal = Ordinal };
}

public static partial class Extensions
{
    public static IEnumerable<RecipeDirection> ToEntities(
        this IEnumerable<CreateRecipeDirectionDto> recipeIngredients)
        => recipeIngredients.Select(direction => direction.ToEntity());
}
