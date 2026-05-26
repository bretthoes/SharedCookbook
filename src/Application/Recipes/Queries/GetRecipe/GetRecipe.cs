using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Queries.GetRecipe;

public sealed record GetRecipeQuery(int Id) : IRequest<RecipeDetailedDto>;

public sealed class GetRecipeQueryHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<GetRecipeQuery, RecipeDetailedDto>
{
    public async Task<RecipeDetailedDto> Handle(GetRecipeQuery request, CancellationToken ct = default)
    {
        var dto = await context.Recipes.GetDetailedDtoById(request.Id, options.Value.ImageBaseUrl, ct)
            ?? throw new NotFoundException(key: request.Id.ToString(), nameof(Recipe));

        if (string.IsNullOrWhiteSpace(dto.AuthorId))
            return dto;

        dto.AuthorEmail = await identityService.GetEmailAsync(dto.AuthorId, ct);
        dto.Author = await identityService.GetDisplayNameAsync(dto.AuthorId, ct);

        return dto;
    }
}
