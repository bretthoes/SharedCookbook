namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(int Id) : IRequest;

public class DeleteRecipeCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteRecipeCommand>
{
    public async Task Handle(DeleteRecipeCommand command, CancellationToken ct = default)
    {
        var recipe = await context.Recipes.FindOrThrowAsync(command.Id, ct);

        if (!await CanDeleteRecipe(recipe.CookbookId, ct))
            throw new ForbiddenAccessException();

        context.Recipes.Remove(recipe);
        recipe.AddDomainEvent(new RecipeDeletedEvent(recipe.Id));

        await context.SaveChangesAsync(ct);
    }

    private async Task<bool> CanDeleteRecipe(int cookbookId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return false;

        var actor = await context.CookbookMemberships.FindForUserAsync(cookbookId, user.Id, ct);

        return actor is not null && actor.Permissions.CanDeleteRecipe;
    }
}
