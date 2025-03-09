using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

internal static class GetRecipesWithPaginationExtensions
{
    public static Task<PaginatedList<RecipeBriefDto>> QueryRecipesInCookbook(this IQueryable<Recipe> recipes,
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
                .ProjectToBriefDtos()
                .PaginatedListAsync(page, pageSize, cancellationToken);

    private static IQueryable<Recipe> HasCookbookId(this IQueryable<Recipe> query, int cookbookId)
        => query
            .Where(recipe => recipe.CookbookId == cookbookId);

    private static IQueryable<Recipe> TitleContains(this IQueryable<Recipe> query, string? search)
        => string.IsNullOrWhiteSpace(search) 
            ? query 
            : query
                .Where(recipe => recipe.Title
                    .ToLower()
                    .Contains(search
                        .Trim()
                        .ToLower()));

    private static IQueryable<RecipeBriefDto> ProjectToBriefDtos(this IQueryable<Recipe> query)
        => query
            .Select(recipe => new RecipeBriefDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
            });

    private static IQueryable<Recipe> OrderByTitle(this IQueryable<Recipe> query)
        => query
            .OrderBy(recipe => recipe.Title);
}
