using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
// TODO move dto here
using SharedCookbook.Application.Recipes.Queries.GetRecipe;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;

public record GetRecipesWithPaginationQuery : IRequest<PaginatedList<RecipeDetailedDto>>
{
    public required int CookbookId { get; set; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetRecipesWithPaginationQueryHandler : IRequestHandler<GetRecipesWithPaginationQuery, PaginatedList<RecipeDetailedDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetRecipesWithPaginationQueryHandler(IApplicationDbContext context, IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<PaginatedList<RecipeDetailedDto>> Handle(GetRecipesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var recipes = await _context.Recipes
            .Where(r => r.CookbookId == request.CookbookId)
            .Include(r => r.Directions)
            .Include(r => r.Ingredients)
            .Include(r => r.Images)
            .OrderBy(c => c.Title)
            .ProjectTo<RecipeDetailedDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        // TODO fix this - performance issue to run for each recipe. Need to either include the author entity to get the name or add a service call to get all distinct usernames and map after the singular query.
        foreach (var recipe in recipes.Items)
        {
            recipe.Author = await _identityService.GetUserNameAsync(recipe.AuthorId.ToString()) ?? string.Empty;
        }

        return recipes;
    }
}
