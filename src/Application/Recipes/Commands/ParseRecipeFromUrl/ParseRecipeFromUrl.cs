using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Recipes.Commands.ParseRecipeFromUrl;

public record ParseRecipeFromUrlCommand(string Url) : IRequest<CreateRecipeDto>;

public class ParseRecipeCommandHandler(IRecipeUrlParser recipeUrlParser)
    : IRequestHandler<ParseRecipeFromUrlCommand, CreateRecipeDto>
{
    
    public Task<CreateRecipeDto> Handle(ParseRecipeFromUrlCommand request, CancellationToken cancellationToken)
    {
        return recipeUrlParser.Parse(request.Url, cancellationToken);
    }
}
