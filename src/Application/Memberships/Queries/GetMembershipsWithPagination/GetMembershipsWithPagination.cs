using AutoMapper.QueryableExtensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public record GetMembershipsWithPaginationQuery : IRequest<PaginatedList<MembershipDto>>
{
    public required int CookbookId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetMembershipsWithPaginationQueryHandler : IRequestHandler<GetMembershipsWithPaginationQuery, PaginatedList<MembershipDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetMembershipsWithPaginationQueryHandler(IApplicationDbContext context, IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<PaginatedList<MembershipDto>> Handle(GetMembershipsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var cookbookMembers = await _context.CookbookMembers
            .AsNoTracking()
            .Where(member => member.CookbookId == request.CookbookId)
            .Select(member => new MembershipDto
            {
                Id = member.Id,
                PersonId = member.CreatedBy ?? 0,
                IsCreator = member.IsCreator,
                CanAddRecipe = member.CanAddRecipe,
                CanUpdateRecipe = member.CanUpdateRecipe,
                CanDeleteRecipe = member.CanDeleteRecipe,
                CanSendInvite = member.CanSendInvite,
                CanRemoveMember = member.CanRemoveMember,
                CanEditCookbookDetails = member.CanEditCookbookDetails,
            })
            .Skip(request.PageNumber - 1)
            .Take(request.PageSize).OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var count = await _context.CookbookMembers
            .AsNoTracking()
            .Where(member => member.CookbookId == request.CookbookId)
            .CountAsync(cancellationToken);

        var userNames = await _identityService
            .GetUserNamesAsync(cookbookMembers.Select(m => m.PersonId).Distinct());

        foreach (var member in cookbookMembers)
        {
            member.Name = userNames[member.PersonId];
        }

        return new PaginatedList<MembershipDto>(cookbookMembers, count, request.PageNumber, request.PageSize);
    }
}
