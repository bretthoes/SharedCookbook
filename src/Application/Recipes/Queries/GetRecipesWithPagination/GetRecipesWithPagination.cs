using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery : IRequest<PaginatedList<RecipeBriefDto>>
{
    public required int CookbookId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRecipesWithPaginationQueryHandler : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRecipesWithPaginationQueryHandler(IApplicationDbContext context, IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<PaginatedList<RecipeBriefDto>> Handle(GetRecipesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return _context.Recipes
            .Where(r => r.CookbookId == request.CookbookId)
            .OrderBy(c => c.Title)
            .ProjectTo<RecipeBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
