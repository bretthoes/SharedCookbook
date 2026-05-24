using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public sealed record UpdateRecipeCommand(UpdateRecipeDto Recipe) : IRequest<int>;

public sealed class UpdateRecipeCommandHandler(IApplicationDbContext context, IOptions<ImageUploadOptions> options)
    : IRequestHandler<UpdateRecipeCommand, int>
{
    public async Task<int> Handle(UpdateRecipeCommand command, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes
                         .AsSplitQuery() // TODO verify this improves performance; a recipe can only have so many directions, images, ingredients, etc. Find max, avg, and suppress warning if the extra round trips slow down query
                         .Include(navigationPropertyPath: recipe => recipe.IngredientSections)
                         .ThenInclude(section => section.Ingredients)
                         .Include(navigationPropertyPath: recipe => recipe.Directions)
                         .Include(navigationPropertyPath: recipe => recipe.Images)
                         .Include(navigationPropertyPath: recipe => recipe.Nutrition)
                         .FirstOrDefaultAsync(recipe => recipe.Id == command.Recipe.Id, cancellationToken)
                     ?? throw new NotFoundException(key: command.Recipe.Id.ToString(), nameof(Recipe));

        // Update primitive properties
        recipe.Title = command.Recipe.Title;
        recipe.Summary = command.Recipe.Summary;
        recipe.Thumbnail = command.Recipe.Thumbnail;
        recipe.VideoPath = command.Recipe.VideoPath;
        recipe.Timing = new Timing(
            command.Recipe.PreparationTimeInMinutes,
            command.Recipe.CookingTimeInMinutes,
            command.Recipe.BakingTimeInMinutes);
        recipe.Servings = command.Recipe.Servings;
        recipe.DietaryTags = new DietaryTags(
            command.Recipe.IsVegetarian,
            command.Recipe.IsVegan,
            command.Recipe.IsGlutenFree,
            command.Recipe.IsDairyFree,
            command.Recipe.IsHealthy,
            command.Recipe.IsCheap,
            command.Recipe.IsLowFodmap,
            command.Recipe.IsHighProtein);
        recipe.MealTypes = new MealTypes(
            command.Recipe.IsBreakfast,
            command.Recipe.IsLunch,
            command.Recipe.IsDinner,
            command.Recipe.IsDessert,
            command.Recipe.IsSnack);

        ReplaceCollection(recipe.IngredientSections, newCollection: command.Recipe.IngredientSections.ToEntities());
        ReplaceCollection(recipe.Directions, newCollection: command.Recipe.Directions.ToEntities(options.Value.ImageBaseUrl));
        ReplaceCollection(recipe.Images, newCollection: command.Recipe.Images.ToEntities(options.Value.ImageBaseUrl));

        recipe.AddDomainEvent(new RecipeUpdatedEvent(recipe.Id));

        await context.SaveChangesAsync(cancellationToken);

        return recipe.Id;
    }

    // TODO find out if this is necessary. Can we replace a collection in EF Core without having to load it in memory?
    private static void ReplaceCollection<T>(ICollection<T> existingCollection, IEnumerable<T> newCollection)
        where T : class
    {
        existingCollection.Clear();
        foreach (var item in newCollection)
        {
            existingCollection.Add(item);
        }
    }
}
