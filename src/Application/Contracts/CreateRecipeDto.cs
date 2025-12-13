namespace SharedCookbook.Application.Contracts;

public sealed record CreateRecipeDto : RecipeDto
{
    public required int CookbookId { get; init; }
}
