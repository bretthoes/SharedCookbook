using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public record GetCookbooksWithPaginationQuery : IRequest<PaginatedList<CookbookBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCookbooksWithPaginationQueryHandler : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _user;

    public GetCookbooksWithPaginationQueryHandler(IApplicationDbContext context, IUser user, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _user = user;
    }

    public Task<PaginatedList<CookbookBriefDto>> Handle(GetCookbooksWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // TODO select dto directly to avoid using Include
        return _context.Cookbooks
            .Where(c => _context.CookbookMembers
                .Any(cm => cm.PersonId == _user.Id && cm.CookbookId == c.Id))
            .OrderBy(c => c.Title)
            .Include(c => c.CookbookMembers)
            .ProjectTo<CookbookBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
