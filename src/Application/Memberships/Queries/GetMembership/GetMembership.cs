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

    public async Task<MembershipDto> Handle(GetMembershipQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.CookbookMemberships.FindAsync([request.Id], cancellationToken) 
            ?? throw new NotFoundException(request.Id.ToString(), nameof(CookbookMembership));

        var dto = new MembershipDto
        {
            CanAddRecipe = entity.CanAddRecipe,
            IsCreator = entity.IsCreator,
            CanUpdateRecipe = entity.CanUpdateRecipe,
            CanDeleteRecipe = entity.CanDeleteRecipe,
            CanRemoveMember = entity.CanRemoveMember,
            CanSendInvite = entity.CanSendInvite,
            CanEditCookbookDetails = entity.CanEditCookbookDetails,
            Name = await _identityService.GetDisplayNameAsync(entity.CreatedBy ?? string.Empty),
            Email = await _identityService.GetEmailAsync(entity.CreatedBy ?? string.Empty)
        };

        Guard.Against.NotFound(request.Id, dto);

        return dto;
    }
}
