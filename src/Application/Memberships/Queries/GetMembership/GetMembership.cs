namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public sealed record GetMembershipQuery(Guid Id) : IRequest<MembershipDto>;

public sealed class GetMembershipQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMembershipQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(GetMembershipQuery query, CancellationToken ct = default)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(query.Id, ct);

        return new MembershipDto
        {
            Id   = membership.Id,
            Tier = membership.Tier,
            Name = membership.DisplayName,
        };
    }
}
