namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;

public sealed record ParseRecipeFromUrlCommand(string Url) : IRequest<CreateRecipeDto>;

public sealed class ParseRecipeCommandHandler(IRecipeUrlParser recipeUrlParser)
    : IRequestHandler<ParseRecipeFromUrlCommand, CreateRecipeDto>
{
    public Task<CreateRecipeDto> Handle(ParseRecipeFromUrlCommand request, CancellationToken cancellationToken)
    {
        return recipeUrlParser.Parse(request.Url, cancellationToken);
    }
}
