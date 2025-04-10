using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery : IRequest<PaginatedList<RecipeDetailedDto>>
{
    public required int CookbookId { get; init; }
    public string? Search { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRecipesWithPaginationQueryHandler : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeDetailedDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityUserRepository _repository;

    public GetRecipesWithPaginationQueryHandler(IApplicationDbContext context, IIdentityUserRepository repository)
    {
        _context = context;
        _repository = repository;
    }

    public Task<PaginatedList<RecipeDetailedDto>> Handle(GetRecipesWithPaginationQuery query,
        CancellationToken cancellationToken)
        => _repository.GetRecipes(query, cancellationToken);
}
