namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

public sealed record GetMembershipByCookbookQuery(int CookbookId) : IRequest<MembershipDto>;

public sealed class GetMembershipByCookbookAndEmailQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetMembershipByCookbookQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(
        GetMembershipByCookbookQuery query,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        var membership = await context.CookbookMemberships.GetByCookbookAndUser(query.CookbookId, user.Id, ct);

        return new MembershipDto
        {
            Id   = membership.Id,
            Tier = membership.Tier,
            Name = membership.DisplayName,
        };
    }
}
