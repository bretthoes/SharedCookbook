using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public record GetCookbooksWithPaginationQuery : IRequest<PaginatedList<CookbookBriefDto>>
{
    public int CreatorPersonId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCookbooksWithPaginationQueryHandler : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCookbooksWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CookbookBriefDto>> Handle(GetCookbooksWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cookbooks
            .Where(x => x.CreatorPersonId == request.CreatorPersonId)
            .OrderBy(x => x.Title)
            .ProjectTo<CookbookBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
