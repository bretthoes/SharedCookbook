namespace SharedCookbook.Application.Contracts;

public sealed record UpdateRecipeDto : RecipeDto
{
    public required Guid Id { get; init; }
}
