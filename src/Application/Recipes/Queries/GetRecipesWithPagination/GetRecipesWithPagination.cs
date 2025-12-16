namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery(
    int CookbookId,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<RecipeBriefDto>>;

public class GetRecipesWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeBriefDto>>
{
    public Task<PaginatedList<RecipeBriefDto>> Handle(GetRecipesWithPaginationQuery query,
        CancellationToken token)
        => context.Recipes.GetBriefRecipeDtos(query.CookbookId, query.PageNumber, query.PageSize, token);
}
