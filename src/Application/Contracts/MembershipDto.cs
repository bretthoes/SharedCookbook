using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Contracts;

public sealed record MembershipDto
{
    public Guid Id { get; set; }

    public string? Name { get; init; }

    public required MembershipTier Tier { get; set; }
}
