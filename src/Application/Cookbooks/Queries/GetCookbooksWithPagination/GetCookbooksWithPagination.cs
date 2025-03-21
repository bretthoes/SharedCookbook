using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public record GetCookbooksWithPaginationQuery : IRequest<PaginatedList<CookbookBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCookbooksWithPaginationQueryHandler(IIdentityUserRepository repository)
    : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    public Task<PaginatedList<CookbookBriefDto>> Handle(
        GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken)
        => repository.GetCookbooks(query, cancellationToken);
}
