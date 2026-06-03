namespace SharedCookbook.Application.Contracts;

public sealed record CreateRecipeDto : RecipeDto
{
    public required Guid CookbookId { get; init; }
}
