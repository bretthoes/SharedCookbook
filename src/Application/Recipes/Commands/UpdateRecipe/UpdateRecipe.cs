using SharedCookbook.Application.Recipes.Commands.CreateRecipe;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public sealed record UpdateRecipeCommand(UpdateRecipeDto Recipe) : IRequest<int>;

public sealed class UpdateRecipeCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateRecipeCommand, int>
{
    public async Task<int> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes
                         .Include(navigationPropertyPath: recipe => recipe.Ingredients)
                         .Include(navigationPropertyPath: recipe => recipe.Directions)
                         .Include(navigationPropertyPath: recipe => recipe.Images)
                         .Include(navigationPropertyPath: recipe => recipe.Nutrition)
                         .Include(navigationPropertyPath: recipe => recipe.IngredientCategories)
                         .FirstOrDefaultAsync(recipe => recipe.Id == command.Recipe.Id, cancellationToken)
                     ?? throw new NotFoundException(key: command.Recipe.Id.ToString(), nameof(Recipe));

        // Update primitive properties
        if (!string.IsNullOrWhiteSpace(command.Recipe.Title)) recipe.Title = command.Recipe.Title;
        recipe.Summary = command.Recipe.Summary;
        recipe.Thumbnail = command.Recipe.Thumbnail;
        recipe.VideoPath = command.Recipe.VideoPath;
        recipe.PreparationTimeInMinutes = command.Recipe.PreparationTimeInMinutes;
        recipe.CookingTimeInMinutes = command.Recipe.CookingTimeInMinutes;
        recipe.BakingTimeInMinutes = command.Recipe.BakingTimeInMinutes;
        recipe.Servings = command.Recipe.Servings;

        // Update collections
        UpdateCollection(recipe.Ingredients, newCollection: command.Recipe.Ingredients.ToEntities());
        UpdateCollection(recipe.Directions, newCollection: command.Recipe.Directions.ToEntities());
        UpdateCollection(recipe.Images, newCollection: command.Recipe.Images.ToEntities());

        recipe.AddDomainEvent(new RecipeUpdatedEvent(recipe.Id));

        await context.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }

    // TODO find out if this is necessary. Can we replace a collection in EF Core without having to load it in memory?
    private static void UpdateCollection<T>(ICollection<T> existingCollection, IEnumerable<T> newCollection)
        where T : class
    {
        existingCollection.Clear();
        foreach (var item in newCollection)
        {
            existingCollection.Add(item);
        }
    }
}
