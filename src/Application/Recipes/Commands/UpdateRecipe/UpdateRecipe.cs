namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand(Recipe Recipe) : IRequest<int>;

public class UpdateRecipeCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<UpdateRecipeCommand, int>
{
    public async Task<int> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes
            .Include(navigationPropertyPath: recipe => recipe.Ingredients)
            .Include(navigationPropertyPath: recipe => recipe.Directions)
            .Include(navigationPropertyPath: recipe => recipe.Images)
            .Include(navigationPropertyPath: recipe => recipe.Nutrition)
            .Include(navigationPropertyPath: recipe => recipe.IngredientCategories)
            .FirstOrDefaultAsync(recipe => recipe.Id == request.Recipe.Id, cancellationToken);

        Guard.Against.NotFound(request.Recipe.Id, recipe);

        // Update primitive properties
        recipe.Title = request.Recipe.Title;
        recipe.Summary = request.Recipe.Summary;
        recipe.Thumbnail = request.Recipe.Thumbnail;
        recipe.VideoPath = request.Recipe.VideoPath;
        recipe.PreparationTimeInMinutes = request.Recipe.PreparationTimeInMinutes;
        recipe.CookingTimeInMinutes = request.Recipe.CookingTimeInMinutes;
        recipe.BakingTimeInMinutes = request.Recipe.BakingTimeInMinutes;
        recipe.Servings = request.Recipe.Servings;

        // Update collections
        UpdateCollection(recipe.Ingredients, newCollection: request.Recipe.Ingredients);
        UpdateCollection(recipe.Directions, newCollection: request.Recipe.Directions);
        UpdateCollection(recipe.Images, newCollection: request.Recipe.Images);
        UpdateCollection(recipe.IngredientCategories, newCollection: request.Recipe.IngredientCategories);

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
