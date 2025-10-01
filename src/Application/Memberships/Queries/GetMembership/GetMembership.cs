namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public record GetMembershipQuery(int Id) : IRequest<MembershipDto>;

public class GetMembershipQueryHandler : IRequestHandler<GetMembershipQuery, MembershipDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetMembershipQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<MembershipDto> Handle(GetMembershipQuery query, CancellationToken cancellationToken)
    {
        var membership = await _context.CookbookMemberships.FindOrThrowAsync(query.Id, cancellationToken);

        var dto = new MembershipDto
        {
            CanAddRecipe = membership.CanAddRecipe,
            IsCreator = membership.IsCreator,
            CanUpdateRecipe = membership.CanUpdateRecipe,
            CanDeleteRecipe = membership.CanDeleteRecipe,
            CanRemoveMember = membership.CanRemoveMember,
            CanSendInvite = membership.CanSendInvite,
            CanEditCookbookDetails = membership.CanEditCookbookDetails,
            Name = await _identityService.GetDisplayNameAsync(membership.CreatedBy ?? string.Empty),
            Email = await _identityService.GetEmailAsync(membership.CreatedBy ?? string.Empty)
        };

        return dto;
    }
}
