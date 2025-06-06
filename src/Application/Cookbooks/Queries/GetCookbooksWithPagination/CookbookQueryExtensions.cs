﻿using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;

public static class CookbookQueryExtensions
{
    public static IQueryable<Cookbook> QueryCookbooksForMember(
        this IQueryable<Cookbook> cookbooks,
        IApplicationDbContext context,
        string? userId)
        => cookbooks
            .AsNoTracking()
            .UserCookbooks(context, userId)
            .OrderByTitle();
            

    private static IQueryable<Cookbook> UserCookbooks(
        this IQueryable<Cookbook> query,
        IApplicationDbContext context,
        string? userId)
        => query
            .Where(c => context.CookbookMemberships
                .Any(cm => cm.CreatedBy == userId && cm.CookbookId == c.Id));

    public static IQueryable<Cookbook> OrderByTitle(this IQueryable<Cookbook> query)
        => query.OrderBy(c => c.Title);
}
