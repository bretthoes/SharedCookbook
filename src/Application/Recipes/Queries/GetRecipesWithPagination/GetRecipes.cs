namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesQuery(int CookbookId, string? Search = null, int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedList<RecipeBriefDto>>;

public class GetRecipesWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRecipesQuery, PaginatedList<RecipeBriefDto>>
{
    public Task<PaginatedList<RecipeBriefDto>> Handle(GetRecipesQuery query,
        CancellationToken token)
        => context.Recipes.QueryBriefDtos(query.CookbookId, query.PageNumber, query.PageSize, token);
}
