using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

public record GetMembershipsWithPaginationQuery : IRequest<PaginatedList<MembershipBriefDto>>
{
    public required int CookbookId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

public class GetMembershipsWithPaginationQueryHandler : IRequestHandler<GetMembershipsWithPaginationQuery, PaginatedList<MembershipBriefDto>>
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

    public async Task<PaginatedList<MembershipBriefDto>> Handle(GetMembershipsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var cookbookMembers = await _context.CookbookMembers
            .AsNoTracking()
            .Where(member => member.CookbookId == request.CookbookId)
            .Select(member => new MembershipBriefDto
            {
                Id = member.Id,
                UserId = member.PersonId,
                IsCreator = member.IsCreator
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

        var userNames = await _identityService
            .GetUserNamesAsync(cookbookMembers.Items.Select(m => m.UserId).Distinct());

        foreach (var member in cookbookMembers.Items)
        {
            member.Name = userNames[member.UserId];
        }

        return cookbookMembers;
    }
}
