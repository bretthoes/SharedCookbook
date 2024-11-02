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
    private readonly IUser _user;

    public GetCookbooksWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public Task<PaginatedList<CookbookBriefDto>> Handle(GetCookbooksWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return _context.Cookbooks
            .AsNoTracking()
            .Where(c => _context.CookbookMembers
                .Any(cm => cm.CreatedBy == _user.Id && cm.CookbookId == c.Id))
            .OrderBy(c => c.Title)
            .Select(c => new CookbookBriefDto
            {
                Id = c.Id,
                Title = c.Title,
                Image = c.Image,
                MembersCount = c.CookbookMembers.Count
            })
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
