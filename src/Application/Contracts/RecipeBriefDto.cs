namespace SharedCookbook.Application.Contracts;

public sealed record RecipeBriefDto
{
    public required int Id { get; init; }

    public required string Title { get; init; }
}
