namespace SharedCookbook.Application.Contracts;

public sealed record InvitationDto
{
    public Guid Id { get; init; }
    
    public Guid? CookbookId { get; init; }

    public string? SenderName { get; init; } = string.Empty;

    public required string CookbookTitle { get; init; }

    public string? CookbookImage { get; init; }

    public DateTimeOffset Created { get; init; }
}
