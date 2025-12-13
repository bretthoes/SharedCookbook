using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public static class GetRecipesWithPaginationExtensions
{
    public static Task<PaginatedList<Recipe>> QueryRecipesInCookbook(this IQueryable<Recipe> recipes,
        int cookbookId,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
            => recipes
                .AsNoTracking()
                .HasCookbookId(cookbookId)
                .TitleContains(search)
                .OrderByTitle()
                .PaginatedListAsync(page, pageSize, cancellationToken);

    public static IQueryable<Recipe> HasCookbookId(this IQueryable<Recipe> query, int cookbookId)
        => query
            .Where(recipe => recipe.CookbookId == cookbookId);

    public static IQueryable<Recipe> TitleContains(this IQueryable<Recipe> query, string? search)
        => string.IsNullOrWhiteSpace(search) 
            ? query 
            : query
                .Where(recipe => recipe.Title
                    .ToLower()
                    .Contains(search
                        .Trim()
                        .ToLower()));


    public static IQueryable<Recipe> OrderByTitle(this IQueryable<Recipe> query)
        => query
            .OrderBy(recipe => recipe.Title);
}
