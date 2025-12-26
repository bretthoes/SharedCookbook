namespace SharedCookbook.Application.Contracts;

public sealed record RecipeIngredientDto
{
    public int Id { get; init; }
    
    public required string Name { get; init; }

    public required bool Optional { get; init; }

    public required int Ordinal { get; init; }
}
