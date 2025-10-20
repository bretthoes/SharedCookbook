namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public class CreateRecipeImageDto
{
    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    public RecipeImage ToEntity() => new() { Name = Name, Ordinal = Ordinal };
}

public static partial class Extensions
{
    public static IEnumerable<RecipeImage> ToEntities(
        this IEnumerable<CreateRecipeImageDto> recipeImages)
        => recipeImages.Select(image => image.ToEntity());
}
