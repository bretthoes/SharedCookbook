namespace SharedCookbook.Application.Memberships.Queries;

public record MembershipDto
{
    public int Id { get; set; }

    public string? Name { get; init; }

    public string? Email { get; set; }

    public required bool IsCreator { get; set; }

    public required bool CanAddRecipe { get; set; }

    public required bool CanUpdateRecipe { get; set; }

    public required bool CanDeleteRecipe { get; set; }

    public required bool CanSendInvite { get; set; }

    public required bool CanRemoveMember { get; set; }

    public required bool CanEditCookbookDetails { get; set; }
}
