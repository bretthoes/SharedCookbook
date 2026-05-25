namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(int Id) : IRequest;
public class DeleteRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteRecipeCommand>
{
    public async Task Handle(DeleteRecipeCommand command, CancellationToken ct = default)
    {
        var recipe = await context.Recipes.FindOrThrowAsync(command.Id, ct);

        context.Recipes.Remove(recipe);

        recipe.AddDomainEvent(new RecipeDeletedEvent(recipe.Id));

        await context.SaveChangesAsync(ct);
    }
}
