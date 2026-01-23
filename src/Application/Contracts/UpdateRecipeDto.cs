namespace SharedCookbook.Application.Contracts;

public sealed record UpdateRecipeDto : RecipeDto
{
    public required int Id { get; init; }
}
