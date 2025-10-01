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
            CanAddRecipe = membership.CanAddRecipe,
            IsCreator = membership.IsCreator,
            CanUpdateRecipe = membership.CanUpdateRecipe,
            CanDeleteRecipe = membership.CanDeleteRecipe,
            CanRemoveMember = membership.CanRemoveMember,
            CanSendInvite = membership.CanSendInvite,
            CanEditCookbookDetails = membership.CanEditCookbookDetails,
            Name = userDto.DisplayName,
            Email = userDto.Email
        };

        return dto;
    }
}
