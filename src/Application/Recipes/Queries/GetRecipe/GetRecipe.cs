namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public class GetRecipeQueryHandler(IApplicationDbContext context, IIdentityService identityService,  IUser user)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var dto = await context.Recipes
            .QueryRecipeDetailedDto(request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, dto);

        // Map the author name separately since this field (ApplicationUser.DisplayName)
        // cannot be accessed through a relationship. This is a byproduct of not having
        // the ApplicationUser object in the domain layer. For collections, this will
        // typically be handled by the IdentityUserRepository. For a single entity like
        // this, it is manually mapped on the line below.
        if (user.Id is null) return dto;

        dto.AuthorEmail = await identityService.GetEmailAsync(user.Id);
        dto.Author = await identityService.GetDisplayNameAsync(user.Id);

        return dto;
    }
}
