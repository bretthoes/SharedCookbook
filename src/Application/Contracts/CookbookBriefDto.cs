namespace SharedCookbook.Application.Contracts;

public sealed record CookbookBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }

    public string? Image { get; init; }

    public int MembersCount { get; init; }
    
    public int RecipeCount { get; init; }
}
