using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Performance;
using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Recipes.Commands.ApplyRecipeEditFromPrompt;

[LongRunningRequest(LongRunningRequestCategory.ExternalApi)]
public sealed record ApplyRecipeEditFromPromptCommand(
    Guid RecipeId,
    string Prompt,
    UpdateRecipeDto Recipe) : IRequest<UpdateRecipeDto>;

public sealed class ApplyRecipeEditFromPromptCommandHandler(
    IApplicationDbContext context,
    IUser user,
    IAiRecipeEditor aiRecipeEditor)
    : IRequestHandler<ApplyRecipeEditFromPromptCommand, UpdateRecipeDto>
{
    public async Task<UpdateRecipeDto> Handle(
        ApplyRecipeEditFromPromptCommand request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var recipe = await context.Recipes
                         .FirstOrDefaultAsync(r => r.Id == request.RecipeId, ct)
                     ?? throw new NotFoundException(key: request.RecipeId.ToString(), nameof(Recipe));

        var actorMembership = await context.CookbookMemberships.FindForUserAsync(recipe.CookbookId, user.Id, ct);

        if (actorMembership is null || !actorMembership.CanUpdateRecipe(recipe))
            throw new ForbiddenAccessException();

        return await aiRecipeEditor.ApplyEditAsync(request.Recipe, request.Prompt, ct);
    }
}
