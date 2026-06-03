namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesQuery(Guid CookbookId, string? Search = null, int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedList<RecipeBriefDto>>;

public class GetRecipesWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRecipesQuery, PaginatedList<RecipeBriefDto>>
{
    public Task<PaginatedList<RecipeBriefDto>> Handle(
        GetRecipesQuery query,
        CancellationToken ct = default) =>
        context.Recipes.QueryBriefDtos(query.CookbookId, query.PageNumber, query.PageSize, ct);
}
