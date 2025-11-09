namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(int Id) : IRequest;
public class DeleteRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteRecipeCommand>
{
    public async Task Handle(DeleteRecipeCommand command, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes.FindOrThrowAsync(command.Id, cancellationToken);

        context.Recipes.Remove(recipe);

        recipe.AddDomainEvent(new RecipeDeletedEvent(recipe.Id));

        await context.SaveChangesAsync(cancellationToken);
    }
}
