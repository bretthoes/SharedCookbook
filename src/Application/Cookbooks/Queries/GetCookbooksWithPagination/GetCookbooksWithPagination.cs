using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public sealed record GetCookbooksWithPaginationQuery(int PageNumber = 1, int PageSize = 10)
    : IRequest<PaginatedList<CookbookBriefDto>>;

public sealed class GetCookbooksWithPaginationQueryHandler(
    IApplicationDbContext context,
    IUser user,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<GetCookbooksWithPaginationQuery, PaginatedList<CookbookBriefDto>>
{
    public Task<PaginatedList<CookbookBriefDto>> Handle(GetCookbooksWithPaginationQuery query,
        CancellationToken ct = default)
        => context.Cookbooks
            .AsNoTracking()
            .QueryBriefDtos(user.Id!, options.Value.ImageBaseUrl, query.PageNumber, query.PageSize, ct);
}
