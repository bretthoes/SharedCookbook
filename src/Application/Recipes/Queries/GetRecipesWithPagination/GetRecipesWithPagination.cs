using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public abstract record GetRecipesWithPaginationQuery : IRequest<PaginatedList<RecipeBriefDto>>
{
    public required int CookbookId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRecipesWithPaginationQueryHandler : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeBriefDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRecipesWithPaginationQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
    }

    public Task<PaginatedList<RecipeBriefDto>> Handle(GetRecipesWithPaginationQuery request,
        CancellationToken cancellationToken)
        => _context.Recipes.QueryRecipesInCookbook(
            request.CookbookId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
}
