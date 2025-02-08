namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public record UpdateRecipeCommand(Recipe Recipe) : IRequest<int>;

public class UpdateRecipeCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<UpdateRecipeCommand, int>
{
    public async Task<int> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Directions)
            .Include(r => r.Images)
            .Include(r => r.Nutrition)
            .Include(r => r.IngredientCategories)
            .FirstOrDefaultAsync(r => r.Id == request.Recipe.Id, cancellationToken);

        Guard.Against.NotFound(request.Recipe.Id, entity);

        // Update primitive properties
        entity.Title = request.Recipe.Title;
        entity.Summary = request.Recipe.Summary;
        entity.Thumbnail = request.Recipe.Thumbnail;
        entity.VideoPath = request.Recipe.VideoPath;
        entity.PreparationTimeInMinutes = request.Recipe.PreparationTimeInMinutes;
        entity.CookingTimeInMinutes = request.Recipe.CookingTimeInMinutes;
        entity.BakingTimeInMinutes = request.Recipe.BakingTimeInMinutes;
        entity.Servings = request.Recipe.Servings;

        // Update collections
        UpdateCollection(entity.Ingredients, request.Recipe.Ingredients);
        UpdateCollection(entity.Directions, request.Recipe.Directions);
        UpdateCollection(entity.Images, request.Recipe.Images);
        UpdateCollection(entity.IngredientCategories, request.Recipe.IngredientCategories);

        await context.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
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
