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

    public Task<PaginatedList<MembershipBriefDto>> Handle(GetMembershipsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // TODO need to map name for each entity as well; use a join to users
        return _context.CookbookMembers
            .Where(r => r.CookbookId == request.CookbookId)
            .ProjectTo<MembershipBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
