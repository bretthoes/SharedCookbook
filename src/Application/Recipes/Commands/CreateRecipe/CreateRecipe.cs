using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.Recipes.Commands.CreateRecipe;

public sealed record CreateRecipeCommand : IRequest<int>
{
    public required CreateRecipeDto Recipe { get; init; }
}

public sealed class CreateRecipeCommandHandler(
    IApplicationDbContext context,
    IOptions<ImageUploadOptions> options,
    IUser user)
    : IRequestHandler<CreateRecipeCommand, int>
{
    public async Task<int> Handle(CreateRecipeCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var actorMembership = await context.CookbookMemberships.FindForUserAsync(
            command.Recipe.CookbookId,
            user.Id,
            ct);

        if (actorMembership is null || !actorMembership.CanAddRecipe())
            throw new ForbiddenAccessException();

        var entity = new Recipe
        {
            Title = command.Recipe.Title,
            CookbookId = command.Recipe.CookbookId,
            Summary = command.Recipe.Summary,
            Timing = new Timing(
                command.Recipe.PreparationTimeInMinutes,
                command.Recipe.CookingTimeInMinutes,
                command.Recipe.BakingTimeInMinutes),
            Servings = command.Recipe.Servings,
            DietaryTags = new DietaryTags(
                command.Recipe.IsVegetarian,
                command.Recipe.IsVegan,
                command.Recipe.IsGlutenFree,
                command.Recipe.IsDairyFree,
                command.Recipe.IsHealthy,
                command.Recipe.IsCheap,
                command.Recipe.IsLowFodmap,
                command.Recipe.IsHighProtein),
            MealTypes = new MealTypes(
                command.Recipe.IsBreakfast,
                command.Recipe.IsLunch,
                command.Recipe.IsDinner,
                command.Recipe.IsDessert,
                command.Recipe.IsSnack),
            Directions = command.Recipe.Directions.ToEntities(options.Value.ImageBaseUrl).ToList(),
            Images = command.Recipe.Images.ToEntities(options.Value.ImageBaseUrl).ToList(),
            IngredientSections = command.Recipe.IngredientSections.ToEntities().ToList()
        };

        entity.AddDomainEvent(new RecipeCreatedEvent(entity));
        context.Recipes.Add(entity);

        await context.SaveChangesAsync(ct);

        return entity.Id;
    }
}
