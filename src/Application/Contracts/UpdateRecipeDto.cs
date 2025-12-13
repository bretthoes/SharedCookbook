namespace SharedCookbook.Application.Contracts;

public sealed record UpdateRecipeDto : RecipeDto
{
    public int Id { get; init; }
}
