using SharedCookbook.Application.Common.Performance;
using SharedCookbook.Application.RecipeDrafting;

namespace SharedCookbook.Application.RecipeDrafting.Commands.ParseRecipeFromUrl;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ParseRecipeFromUrlCommand(string Url, bool ExtractFromVideo = false) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeCommandHandler(
    IRecipeUrlParser recipeUrlParser)
    : IRequestHandler<ParseRecipeFromUrlCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromUrlCommand request, CancellationToken ct = default)
    {
        if (YouTubeImportUrls.IsLongForm(request.Url))
            throw new UnprocessableContentException(YouTubeImportUrls.LongFormMessage);

        return recipeUrlParser.Parse(request.Url, request.ExtractFromVideo, ct);
    }
}
