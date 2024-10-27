namespace SharedCookbook.Application.Memberships.Queries;

public class MembershipBriefDto
{
    public required string MemberName { get; set; }

    public required bool IsCreator { get; set; }
}
