namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand : IRequest<int>
{
    public required CreateRecipeDto Recipe { get; init; }
}

public sealed class CreateRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateRecipeCommand, int>
{
    public async Task<int> Handle(CreateRecipeCommand command, CancellationToken cancellationToken)
    {
        var entity = new Recipe
        {
            Title = command.Recipe.Title,
            CookbookId = command.Recipe.CookbookId,
            Summary = command.Recipe.Summary,
            Thumbnail = command.Recipe.Thumbnail, // TODO grab first image, compress, and set here. Same for update (if it changes)
            VideoPath = command.Recipe.VideoPath,
            PreparationTimeInMinutes = command.Recipe.PreparationTimeInMinutes,
            CookingTimeInMinutes = command.Recipe.CookingTimeInMinutes,
            BakingTimeInMinutes = command.Recipe.BakingTimeInMinutes,
            Servings = command.Recipe.Servings,
            Directions = command.Recipe.Directions.Select(direction => new RecipeDirection
            {
                Text = direction.Text,
                Ordinal = direction.Ordinal,
                Image = direction.Image,
            }).ToList(),
            Images = command.Recipe.Images.Select(image => new RecipeImage
            {
                Name = image.Name,
                Ordinal = image.Ordinal,
            }).ToList(),
            Ingredients = command.Recipe.Ingredients.Select(ingredient => new RecipeIngredient
            {
                Name = ingredient.Name,
                Ordinal = ingredient.Ordinal,
                Optional = ingredient.Optional,
            }).ToList()
        };

        entity.AddDomainEvent(new RecipeCreatedEvent(entity));

        context.Recipes.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
