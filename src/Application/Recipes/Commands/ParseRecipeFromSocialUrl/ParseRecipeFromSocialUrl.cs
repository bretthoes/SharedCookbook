using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromSocialUrl;

public sealed record ParseRecipeFromSocialUrlCommand(string Url, string Platform) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeFromSocialUrlCommandHandler(
    IRecipeUrlParser recipeUrlParser,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<ParseRecipeFromSocialUrlCommand, CreateRecipeDto>
{
    public async Task<CreateRecipeDto> Handle(
        ParseRecipeFromSocialUrlCommand request,
        CancellationToken cancellationToken)
    {
        var dto = await recipeUrlParser.Parse(request.Url, cancellationToken, extractFromVideo: true);
        return EnrichImagesWithBaseUrl(dto, options.Value.ImageBaseUrl);
    }

    private static CreateRecipeDto EnrichImagesWithBaseUrl(CreateRecipeDto dto, string imageBaseUrl) =>
        dto with
        {
            Images = dto.Images
                .Select(img => img with { Name = img.Name.EnsurePrefixUrl(imageBaseUrl) })
                .ToList(),
        };
}
