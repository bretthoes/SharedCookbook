namespace SharedCookbook.Application.Contracts;

public sealed record RecipeImageDto
{
    public required string Name { get; init; }

    public required int Ordinal { get; init; }
}
