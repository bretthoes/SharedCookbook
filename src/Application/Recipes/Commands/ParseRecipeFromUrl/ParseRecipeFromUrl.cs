namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;

public sealed record ParseRecipeFromUrlCommand(string Url, bool ExtractFromVideo = false) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeCommandHandler(
    IRecipeUrlParser recipeUrlParser)
    : IRequestHandler<ParseRecipeFromUrlCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromUrlCommand request, CancellationToken cancellationToken)
        => recipeUrlParser.Parse(request.Url, cancellationToken, request.ExtractFromVideo);
}
