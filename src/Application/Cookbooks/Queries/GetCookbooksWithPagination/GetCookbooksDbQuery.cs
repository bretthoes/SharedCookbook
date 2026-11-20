using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

internal static class GetCookbooksDbQuery
{
    extension(IQueryable<Cookbook> query)
    {
        internal Task<PaginatedList<CookbookBriefDto>> QueryBriefDtos(
            string userId,
            string imageBaseUrl,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => query
                .ForMember(userId)
                .OrderByTitle()
                .Select(cookbook => new CookbookBriefDto
                {
                    Id           = cookbook.Id,
                    Title        = cookbook.Title,
                    Image        = cookbook.Image != null ? cookbook.Image.EnsurePrefixUrl(imageBaseUrl) : null,
                    MembersCount = cookbook.Memberships.Count,
                    RecipeCount  = cookbook.Recipes.Count,
                })
                .PaginatedListAsync(pageNumber, pageSize, ct);
    }

    extension(IQueryable<Cookbook> cookbooks)
    {
        private IQueryable<Cookbook> ForMember(string? userId)
            => string.IsNullOrWhiteSpace(userId)
                ? cookbooks.Where(_ => false)
                : cookbooks.Where(cookbook => cookbook.Memberships.Any(membership => membership.CreatedBy == userId));

        private IQueryable<Cookbook> OrderByTitle()
            => cookbooks.OrderBy(cookbook => cookbook.Title);
    }
}
