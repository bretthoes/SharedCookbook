namespace SharedCookbook.Application.Contracts;

public class RecipeImageDto
{
    public required string Name { get; init; }

    public required int Ordinal { get; init; }

    public RecipeImage ToEntity() => new() { Name = Name, Ordinal = Ordinal };
}

public static partial class Extensions
{
    public static IEnumerable<RecipeImage> ToEntities(
        this IEnumerable<RecipeImageDto> recipeImages)
        => recipeImages.Select(image => image.ToEntity());
}
