using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;

public sealed record ParseRecipeFromUrlCommand(string Url) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeCommandHandler(
    IRecipeUrlParser recipeUrlParser,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<ParseRecipeFromUrlCommand, CreateRecipeDto>
{
    public async Task<CreateRecipeDto> Handle(ParseRecipeFromUrlCommand request, CancellationToken cancellationToken)
    {
        var dto = await recipeUrlParser.Parse(request.Url, cancellationToken);
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
