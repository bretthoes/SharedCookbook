using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery(
    int CookbookId,
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<RecipeDetailedDto>>;

public class GetRecipesWithPaginationQueryHandler(IIdentityUserRepository repository)
    : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeDetailedDto>>
{
    public Task<PaginatedList<RecipeDetailedDto>> Handle(GetRecipesWithPaginationQuery query,
        CancellationToken cancellationToken)
        => repository.GetRecipes(query, cancellationToken);
}
