using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetRecipeQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var dto = await _context.Recipes
            .AsNoTracking()
            .Select(r => new RecipeDetailedDto
            {
                Id = r.Id,
                Title = r.Title,
                AuthorId = r.CreatedBy ?? 0,
                Summary = r.Summary,
                Thumbnail = r.Thumbnail,
                VideoPath = r.VideoPath,
                PreparationTimeInMinutes = r.PreparationTimeInMinutes,
                CookingTimeInMinutes = r.CookingTimeInMinutes,
                BakingTimeInMinutes = r.BakingTimeInMinutes,
                Servings = r.Servings,
                Directions = r.Directions.Select(d => new RecipeDirectionDto
                {
                    Id = d.Id,
                    Text = d.Text,
                    Ordinal = d.Ordinal,
                    Image = d.Image
                }).ToList(),
                Images = r.Images.Select(i => new RecipeImageDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Ordinal = i.Ordinal
                    
                }).ToList(),
                Ingredients = r.Ingredients.Select(i => new RecipeIngredientDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Ordinal = i.Ordinal,
                    Optional = i.Optional
                }).ToList()
            })
            .SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, dto);

        // Map the author name separately since author cannot be accessed through a relationship.
        // this is a byproduct of not having the ApplicationUser object in the domain layer.
        dto.Author = await _identityService.GetUserNameAsync(dto.AuthorId) ?? string.Empty;

        return dto;
    }
}
