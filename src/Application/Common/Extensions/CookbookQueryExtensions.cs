using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Common.Extensions;

public static class CookbookQueryExtensions
{
    public static IQueryable<Cookbook> ForMember(
            this IQueryable<Cookbook> cookbooks, string? userId)
            => string.IsNullOrWhiteSpace(userId)
               ? cookbooks.Where(_ => false)
               : cookbooks.Where(cookbook => cookbook.Memberships.Any(membership => membership.CreatedBy == userId));

    public static IQueryable<Cookbook> OrderByTitle(this IQueryable<Cookbook> query)
        => query.OrderBy(cookbook => cookbook.Title);
}
