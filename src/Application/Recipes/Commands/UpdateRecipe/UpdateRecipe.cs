namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

// TODO use a dto here; shouldn't tie this to domain model
public record UpdateRecipeCommand(Recipe Recipe) : IRequest<int>;

public class UpdateRecipeCommandHandler(IApplicationDbContext context) 
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
        recipe.Title = command.Recipe.Title;
        recipe.Summary = command.Recipe.Summary;
        recipe.Thumbnail = command.Recipe.Thumbnail;
        recipe.VideoPath = command.Recipe.VideoPath;
        recipe.PreparationTimeInMinutes = command.Recipe.PreparationTimeInMinutes;
        recipe.CookingTimeInMinutes = command.Recipe.CookingTimeInMinutes;
        recipe.BakingTimeInMinutes = command.Recipe.BakingTimeInMinutes;
        recipe.Servings = command.Recipe.Servings;

        // Update collections
        UpdateCollection(recipe.Ingredients, newCollection: command.Recipe.Ingredients);
        UpdateCollection(recipe.Directions, newCollection: command.Recipe.Directions);
        UpdateCollection(recipe.Images, newCollection: command.Recipe.Images);
        UpdateCollection(recipe.IngredientCategories, newCollection: command.Recipe.IngredientCategories);
        
        // TODO add domain event with logging

        await context.SaveChangesAsync(cancellationToken);
        
        return recipe.Id;
    }

    private static void UpdateCollection<T>(ICollection<T> existingCollection, ICollection<T> newCollection)
        where T : class
    {
        existingCollection.Clear();
        foreach (var item in newCollection)
        {
            existingCollection.Add(item);
        }
    }
}
