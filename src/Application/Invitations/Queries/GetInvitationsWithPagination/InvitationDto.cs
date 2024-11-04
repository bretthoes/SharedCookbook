namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public record InvitationDto
{
    public int Id { get; init; }

    public string? SenderName { get; init; } = string.Empty;

    public string? SenderEmail { get; init; } = string.Empty;

    public required string CookbookTitle { get; init; }

    public string? CookbookImage { get; init; }

    public DateTimeOffset Created { get; init; }

    public int? CreatedBy { get; init; }
}
