using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Contracts;

public sealed record MembershipDto
{
    public int Id { get; set; }

    public string? Name { get; init; }

    public string? Email { get; set; }

    public required MembershipTier Tier { get; set; }
}
