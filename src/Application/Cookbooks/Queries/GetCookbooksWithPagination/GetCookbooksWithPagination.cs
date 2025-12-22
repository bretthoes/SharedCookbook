namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public sealed record GetCookbooksWithPaginationQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedList<CookbookBriefDto>>;

public sealed class GetCookbooksWithPaginationQueryHandler(IIdentityRepository repository)
    : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    public Task<PaginatedList<CookbookBriefDto>> Handle(GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken)
        => repository.GetCookbooks(query, cancellationToken);
}
