using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public record GetCookbooksWithPaginationQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedList<CookbookBriefDto>>;

public class GetCookbooksWithPaginationQueryHandler(IIdentityUserRepository repository)
    : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    public Task<PaginatedList<CookbookBriefDto>> Handle(
        GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken)
        => repository.GetCookbooks(query, cancellationToken);
}
