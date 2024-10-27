using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.Memberships.Queries.GetMembership;

public record GetMembershipQuery(int Id) : IRequest<MembershipDetailedDto>;

public class GetMembershipQueryHandler : IRequestHandler<GetMembershipQuery, MembershipDetailedDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetMembershipQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<MembershipDetailedDto> Handle(GetMembershipQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.CookbookMembers.FindAsync([request.Id], cancellationToken) 
            ?? throw new NotFoundException(request.Id.ToString(), nameof(CookbookMember));

        var dto = new MembershipDetailedDto() {
            CanAddRecipe = entity.CanAddRecipe,
            CanUpdateRecipe = entity.CanUpdateRecipe,
            CanDeleteRecipe = entity.CanDeleteRecipe,
            CanRemoveMember = entity.CanRemoveMember,
            CanSendInvite = entity.CanSendInvite,
            CanEditCookbookDetails = entity.CanEditCookbookDetails,
            MemberName = await _identityService.GetUserNameAsync(entity.PersonId.ToString()) ?? string.Empty
        };
        // TODO update query to also check if user is the creator of the cookbook; inclue this property in dto.
        // NOTE the creator will always be the lowest Id member in the cookbook.

        Guard.Against.NotFound(request.Id, dto);

        return dto;
    }
}
