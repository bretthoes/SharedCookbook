namespace SharedCookbook.Application.Memberships.Queries.GetMembershipByCookbook;

public record GetMembershipByCookbookQuery(int CookbookId) : IRequest<MembershipDto>;

public class GetMembershipByCookbookAndEmailQueryHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user)
    : IRequestHandler<GetMembershipByCookbookQuery, MembershipDto>
{
    public async Task<MembershipDto> Handle(
        GetMembershipByCookbookQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
       
        var userDto = await identityService.FindByIdAsync(user.Id) ?? throw new UnauthorizedAccessException();

        var membership = await context.CookbookMemberships.SingleForCookbookAndUser(query.CookbookId, userDto.Id, cancellationToken);
        var dto = new MembershipDto
        {
            Id = membership.Id,
            CanAddRecipe = membership.Permissions.CanAddRecipe,
            IsCreator = membership.IsOwner,
            CanUpdateRecipe = membership.Permissions.CanUpdateRecipe,
            CanDeleteRecipe = membership.Permissions.CanDeleteRecipe,
            CanRemoveMember = membership.Permissions.CanRemoveMember,
            CanSendInvite = membership.Permissions.CanSendInvite,
            CanEditCookbookDetails = membership.Permissions.CanEditCookbookDetails,
            Name = userDto.DisplayName,
            Email = userDto.Email
        };

        return dto;
    }
}
