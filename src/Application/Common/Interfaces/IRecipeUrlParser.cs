using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IRecipeUrlParser
{
    public Task<CreateRecipeDto> Parse(string url, CancellationToken cancellationToken);
}
