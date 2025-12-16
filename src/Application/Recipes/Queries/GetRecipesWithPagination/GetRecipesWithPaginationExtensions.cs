using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public static class GetRecipesWithPaginationDbQuery
{
    public static Task<PaginatedList<RecipeBriefDto>> GetBriefRecipeDtos(this IQueryable<Recipe> query,
        int cookbookId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        => query.AsNoTracking()
            .HasCookbookId(cookbookId)
            .OrderByTitle()
            .ToDtos()
            .PaginatedListAsync(pageNumber, pageSize, cancellationToken);
    
    private static IQueryable<Recipe> HasCookbookId(this IQueryable<Recipe> query, int cookbookId)
        => query
            .Where(recipe => recipe.CookbookId == cookbookId);

    private static IQueryable<Recipe> OrderByTitle(this IQueryable<Recipe> query)
        => query
            .OrderBy(recipe => recipe.Title);
}
