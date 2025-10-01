namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public class GetRecipeQueryHandler(IApplicationDbContext context, IIdentityService identityService,  IUser user)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        var dto = await context.Recipes.QueryRecipeDetailedDto(request.Id, cancellationToken)
            ?? throw new NotFoundException(key: request.Id.ToString(), nameof(Recipe));

        var userDto = await identityService.FindByIdAsync(user.Id);
        
        dto.AuthorEmail = userDto?.Email;
        dto.Author = userDto?.DisplayName;

        return dto;
    }
}
