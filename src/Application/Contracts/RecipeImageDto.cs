namespace SharedCookbook.Application.Contracts;

public sealed record RecipeImageDto
{
    public int Id { get; init; }
    
    public required string Name { get; init; }

    public required int Ordinal { get; init; }
}
