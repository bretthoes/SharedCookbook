using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public record GetCookbooksWithPaginationQuery : IRequest<PaginatedList<CookbookBriefDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCookbooksWithPaginationQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    public Task<PaginatedList<CookbookBriefDto>> Handle(
        GetCookbooksWithPaginationQuery request,
        CancellationToken cancellationToken)
        => context.Cookbooks
            .QueryCookbooksForMember(context,
                user?.Id,
                request.PageNumber,
                request.PageSize,
                cancellationToken);
}
