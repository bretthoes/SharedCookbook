using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery(
    int CookbookId,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<RecipeBriefDto>>;

public class GetRecipesWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeBriefDto>>
{
    public Task<PaginatedList<RecipeBriefDto>> Handle(GetRecipesWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.Recipes.GetBriefRecipes(query.CookbookId, query.PageNumber, query.PageSize, cancellationToken);
}

public static class RecipeQueryExtensions
{
    public static Task<PaginatedList<RecipeBriefDto>> GetBriefRecipes(this IQueryable<Recipe> query,
        int cookbookId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        => query.AsNoTracking()
            .HasCookbookId(cookbookId)
            .OrderByTitle()
            .Select(recipe => new RecipeBriefDto
            {
                Id = recipe.Id,
                Title = recipe.Title
            })
            .PaginatedListAsync(pageNumber, pageSize, cancellationToken);
}

