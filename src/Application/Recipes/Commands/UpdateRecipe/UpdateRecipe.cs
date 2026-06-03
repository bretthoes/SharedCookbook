using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.Recipes.Commands.UpdateRecipe;

public sealed record UpdateRecipeCommand(UpdateRecipeDto Recipe) : IRequest<Guid>;

public sealed class UpdateRecipeCommandHandler(
    IApplicationDbContext context,
    IOptions<ImageUploadOptions> options,
    IUser user)
    : IRequestHandler<UpdateRecipeCommand, Guid>
{
    public async Task<Guid> Handle(UpdateRecipeCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var recipe = await context.Recipes
                         .FirstOrDefaultAsync(recipe => recipe.Id == command.Recipe.Id, ct)
                     ?? throw new NotFoundException(key: command.Recipe.Id.ToString(), nameof(Recipe));

        var actorMembership = await context.CookbookMemberships.FindForUserAsync(recipe.CookbookId, user.Id, ct);

        if (actorMembership is null || !actorMembership.CanUpdateRecipe(recipe))
            throw new ForbiddenAccessException();

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

        recipe.IngredientSections.Clear();
        foreach (var section in command.Recipe.IngredientSections.ToEntities())
            recipe.IngredientSections.Add(section);

        recipe.Directions.Clear();
        foreach (var direction in command.Recipe.Directions.ToEntities(options.Value.ImageBaseUrl))
            recipe.Directions.Add(direction);

        recipe.Images.Clear();
        foreach (var image in command.Recipe.Images.ToEntities(options.Value.ImageBaseUrl))
            recipe.Images.Add(image);

        recipe.AddDomainEvent(new RecipeUpdatedEvent(recipe.Id));

        await context.SaveChangesAsync(ct);

        return recipe.Id;
    }
}
