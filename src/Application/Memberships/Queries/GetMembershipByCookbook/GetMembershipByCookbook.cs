namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

public sealed record GetMembershipByCookbookQuery(int CookbookId) : IRequest<MembershipDto>;

public sealed class GetMembershipByCookbookAndEmailQueryHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<GetMembershipByCookbookQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(
        GetMembershipByCookbookQuery query,
        CancellationToken ct)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        (string? email, string? name) = await identityService.FindByIdAsync(user.Id) 
            ?? throw new UnauthorizedAccessException();

        var membership = await context.CookbookMemberships.GetByCookbookAndUser(query.CookbookId, user.Id, ct);
        
        var dto = new MembershipDto
        {
            Id = membership.Id,
            CanAddRecipe = membership.Permissions.CanAddRecipe,
            IsOwner = membership.IsOwner,
            CanUpdateRecipe = membership.Permissions.CanUpdateRecipe,
            CanDeleteRecipe = membership.Permissions.CanDeleteRecipe,
            CanRemoveMember = membership.Permissions.CanRemoveMember,
            CanSendInvite = membership.Permissions.CanSendInvite,
            CanEditCookbookDetails = membership.Permissions.CanEditCookbookDetails,
            Name = name,
            Email = email
        };

        return dto;
    }
}
