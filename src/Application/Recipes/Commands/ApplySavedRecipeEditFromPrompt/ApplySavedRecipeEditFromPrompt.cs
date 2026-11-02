using SharedCookbook.Application.Common.Performance;

namespace SharedCookbook.Application.Recipes.Commands.ApplySavedRecipeEditFromPrompt;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ApplySavedRecipeEditFromPromptCommand(
    Guid RecipeId,
    string Prompt,
    UpdateRecipeDto Recipe) : IRequest<UpdateRecipeDto>;

public sealed record ApplySavedRecipeEditFromPromptBody(string Prompt, UpdateRecipeDto Recipe);

public sealed class ApplySavedRecipeEditFromPromptCommandHandler(
    IApplicationDbContext context,
    IUser user,
    IAiRecipeEditor aiRecipeEditor)
    : IRequestHandler<ApplySavedRecipeEditFromPromptCommand, UpdateRecipeDto>
{
    public async Task<UpdateRecipeDto> Handle(
        ApplySavedRecipeEditFromPromptCommand request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var recipe = await context.Recipes
                         .FirstOrDefaultAsync(r => r.Id == request.RecipeId, ct)
                     ?? throw new NotFoundException(key: request.RecipeId.ToString(), nameof(Recipe));

        var actorMembership = await context.CookbookMemberships.FindForUserAsync(recipe.CookbookId, user.Id, ct);

        if (actorMembership is null || !actorMembership.CanUpdateRecipe(recipe))
            throw new ForbiddenAccessException();

        var edited = await aiRecipeEditor.ApplyEditAsync(request.Recipe, request.Prompt, ct);
        return ToUpdateRecipeDto(edited, request.RecipeId);
    }

    private static UpdateRecipeDto ToUpdateRecipeDto(RecipeDto edited, Guid id) => new()
    {
        Id = id,
        Title = edited.Title,
        Summary = edited.Summary,
        PreparationTimeInMinutes = edited.PreparationTimeInMinutes,
        CookingTimeInMinutes = edited.CookingTimeInMinutes,
        BakingTimeInMinutes = edited.BakingTimeInMinutes,
        Servings = edited.Servings,
        IsVegetarian = edited.IsVegetarian,
        IsVegan = edited.IsVegan,
        IsGlutenFree = edited.IsGlutenFree,
        IsDairyFree = edited.IsDairyFree,
        IsHealthy = edited.IsHealthy,
        IsCheap = edited.IsCheap,
        IsLowFodmap = edited.IsLowFodmap,
        IsHighProtein = edited.IsHighProtein,
        IsBreakfast = edited.IsBreakfast,
        IsLunch = edited.IsLunch,
        IsDinner = edited.IsDinner,
        IsDessert = edited.IsDessert,
        IsSnack = edited.IsSnack,
        Directions = edited.Directions,
        Images = edited.Images,
        IngredientSections = edited.IngredientSections,
    };
}
