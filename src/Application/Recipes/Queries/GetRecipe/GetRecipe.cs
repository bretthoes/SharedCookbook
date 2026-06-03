using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public sealed record GetRecipeQuery(Guid Id) : IRequest<RecipeDetailedDto>;

public sealed class GetRecipeQueryHandler(
    IApplicationDbContext context,
    IUser user,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);
        
        var dto = await context.Recipes.GetDetailedDtoById(request.Id, options.Value.ImageBaseUrl, ct)
            ?? throw new NotFoundException(key: request.Id.ToString(), nameof(Recipe));

        dto.IsAuthor = IsCallerAuthorOfRecipe(user.Id, dto);

        return dto;
    }
    
    private static bool IsCallerAuthorOfRecipe(string userId, RecipeDetailedDto recipe) =>
        !string.IsNullOrWhiteSpace(recipe.AuthorId) && string.Equals(recipe.AuthorId, userId, StringComparison.Ordinal);
}
