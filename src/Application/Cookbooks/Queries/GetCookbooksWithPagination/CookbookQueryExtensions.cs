using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

internal static class CookbookQueryExtensions
{
    public static Task<PaginatedList<CookbookBriefDto>> QueryCookbooksForMember(
        this IQueryable<Cookbook> cookbooks,
        IApplicationDbContext context,
        string? userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
        => cookbooks
            .AsNoTracking()
            .UserCookbooks(context, userId)
            .OrderByTitle()
            .ProjectToDtos()
            .PaginatedListAsync(page, pageSize, cancellationToken);

    private static IQueryable<Cookbook> UserCookbooks(
        this IQueryable<Cookbook> query,
        IApplicationDbContext context,
        string? userId)
        => query
            .Where(c => context.CookbookMembers
                .Any(cm => cm.CreatedBy == userId && cm.CookbookId == c.Id));

    private static IQueryable<CookbookBriefDto> ProjectToDtos(this IQueryable<Cookbook> query)
        => query.Select(c => new CookbookBriefDto
        {
            Id = c.Id,
            Title = c.Title,
            Image = c.Image,
            MembersCount = c.CookbookMembers.Count
        });

    private static IQueryable<Cookbook> OrderByTitle(this IQueryable<Cookbook> query)
        => query.OrderBy(c => c.Title);
}
