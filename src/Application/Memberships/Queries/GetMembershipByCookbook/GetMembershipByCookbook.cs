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

        // TODO break this query into smaller extensions
        var member = await context.CookbookMemberships
            .SingleAsync(member => member.CookbookId == query.CookbookId && userDto.Id == member.CreatedBy, cancellationToken);

        var dto = new MembershipDto
        {
            Id = member.Id,
            CanAddRecipe = member.CanAddRecipe,
            IsCreator = member.IsCreator,
            CanUpdateRecipe = member.CanUpdateRecipe,
            CanDeleteRecipe = member.CanDeleteRecipe,
            CanRemoveMember = member.CanRemoveMember,
            CanSendInvite = member.CanSendInvite,
            CanEditCookbookDetails = member.CanEditCookbookDetails,
            Name = userDto.DisplayName,
            Email = userDto.Email
        };

        return dto;
    }
}
