using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetRecipeQueryHandler(IApplicationDbContext context, IIdentityService identityService, IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Recipes
           .Include(r => r.Directions)
           .Include(r => r.Ingredients)
           .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        var dto = _mapper.Map<RecipeDetailedDto>(entity);

        dto.Author = await _identityService.GetUserNameAsync(entity.AuthorId.ToString()) ?? string.Empty;

        return dto;
    }
}
