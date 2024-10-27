namespace SharedCookbook.Application.Memberships.Queries;

public class MembershipDetailedDto
{
    public string? MemberName { get; set; }

    public required bool CanAddRecipe { get; set; }

    public required bool CanUpdateRecipe { get; set; }

    public required bool CanDeleteRecipe { get; set; }

    public required bool CanSendInvite { get; set; }

    public required bool CanRemoveMember { get; set; }

    public required bool CanEditCookbookDetails { get; set; }
}
