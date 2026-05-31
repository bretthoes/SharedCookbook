namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public sealed record GetMembershipQuery(int Id) : IRequest<MembershipDto>;

public sealed class GetMembershipQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<GetMembershipQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(GetMembershipQuery query, CancellationToken ct = default)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(query.Id, ct);

        return new MembershipDto
        {
            Id = membership.Id,
            Tier = membership.Tier,
            Name = await identityService.GetDisplayNameAsync(membership.CreatedBy ?? string.Empty, ct),
            Email = await identityService.GetEmailAsync(membership.CreatedBy ?? string.Empty, ct)
        };
    }
}
