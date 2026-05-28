namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(int Id) : IRequest;

public class DeleteRecipeCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteRecipeCommand>
{
    public async Task Handle(DeleteRecipeCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);
        var recipeToDelete = await context.Recipes.FindOrThrowAsync(command.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(recipeToDelete.CookbookId, user.Id, ct);

        if (actorMembership is null || !actorMembership.CanDeleteRecipe(recipeToDelete))
            throw new ForbiddenAccessException();

        context.Recipes.Remove(recipeToDelete);
        recipeToDelete.AddDomainEvent(new RecipeDeletedEvent(recipeToDelete.Id));

        await context.SaveChangesAsync(ct);
    }
}
