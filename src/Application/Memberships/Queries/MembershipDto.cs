namespace SharedCookbook.Application.Memberships.Queries;

public class MembershipDto
{
    public int Id { get; set; }

    public int PersonId { get; set; }

    public string Name { get; set; } = string.Empty;

    public required bool IsCreator { get; set; }

    public required bool CanAddRecipe { get; set; }

    public required bool CanUpdateRecipe { get; set; }

    public required bool CanDeleteRecipe { get; set; }

    public required bool CanSendInvite { get; set; }

    public required bool CanRemoveMember { get; set; }

    public required bool CanEditCookbookDetails { get; set; }
}
