namespace SharedCookbook.Application.Memberships.Queries;

public class MembershipBriefDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public required bool IsCreator { get; set; }
}
