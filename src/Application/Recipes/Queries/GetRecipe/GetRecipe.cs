namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public class GetRecipeQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var dto = await context.Recipes
            .FindRecipeDtoById(request.Id, cancellationToken);
 
        Guard.Against.NotFound(request.Id, dto);

        // Map the author name separately since this field (ApplicationUser.DisplayName)
        // cannot be accessed through a relationship. This is a byproduct of not having
        // the ApplicationUser object in the domain layer. For collections, this will
        // typically be handled by the IdentityUserRepository. For a single entity like
        // this, it is manually mapped on the line below.
        dto.Author = await identityService.GetDisplayNameAsync(dto.AuthorId ?? string.Empty);

        return dto;
    }
}
