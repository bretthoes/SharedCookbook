namespace SharedCookbook.Application.Contracts;

public sealed record RecipeDirectionDto
{
    public required string Text { get; init; }

    public required int Ordinal { get; init; }

    public string? Image { get; init; }
}
