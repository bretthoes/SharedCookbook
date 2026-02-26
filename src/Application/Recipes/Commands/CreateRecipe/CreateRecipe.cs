using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand : IRequest<int>
{
    public required CreateRecipeDto Recipe { get; init; }
}

public sealed class CreateRecipeCommandHandler(
    IApplicationDbContext context,
    IOptions<ImageUploadOptions> options)
    : IRequestHandler<CreateRecipeCommand, int>
{
    public async Task<int> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
    {
        var entity = new Recipe
        {
            Title = command.Recipe.Title,
            CookbookId = command.Recipe.CookbookId,
            Summary = command.Recipe.Summary,
            PreparationTimeInMinutes = command.Recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = command.Recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = command.Recipe.BakingTimeInMinutes,
            Servings = command.Recipe.Servings,
            Directions = command.Recipe.Directions.ToEntities().ToList(),
            Images = command.Recipe.Images.ToEntities(options.Value.ImageBaseUrl).ToList(),
            Ingredients = command.Recipe.Ingredients.ToEntities().ToList()
        };

        entity.AddDomainEvent(new RecipeCreatedEvent(entity));

        context.Recipes.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
