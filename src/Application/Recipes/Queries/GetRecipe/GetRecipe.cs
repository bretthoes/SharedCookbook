using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public sealed record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public sealed class GetRecipeQueryHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IUser user,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        var dto = await context.Recipes.QueryDetailedDto(request.Id, options.Value.ImageBaseUrl, token)
            ?? throw new NotFoundException(key: request.Id.ToString(), nameof(Recipe));

        (string? email, string? name) = await identityService.FindByIdAsync(user.Id)
            ?? throw new UnauthorizedAccessException();

        dto.AuthorEmail = email;
        dto.Author = name;

        return dto;
    }
}
