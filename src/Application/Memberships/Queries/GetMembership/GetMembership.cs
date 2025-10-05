namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public record GetMembershipQuery(int Id) : IRequest<MembershipDto>;

public class GetMembershipQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<GetMembershipQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(GetMembershipQuery query, CancellationToken cancellationToken)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(query.Id, cancellationToken);

        return new MembershipDto
        {
            CanAddRecipe = membership.Permissions.CanAddRecipe,
            IsCreator = membership.IsOwner,
            CanUpdateRecipe = membership.Permissions.CanUpdateRecipe,
            CanDeleteRecipe = membership.Permissions.CanDeleteRecipe,
            CanRemoveMember = membership.Permissions.CanRemoveMember,
            CanSendInvite = membership.Permissions.CanSendInvite,
            CanEditCookbookDetails = membership.Permissions.CanEditCookbookDetails,
            Name = await identityService.GetDisplayNameAsync(membership.CreatedBy ?? string.Empty),
            Email = await identityService.GetEmailAsync(membership.CreatedBy ?? string.Empty)
        };
    }
}
