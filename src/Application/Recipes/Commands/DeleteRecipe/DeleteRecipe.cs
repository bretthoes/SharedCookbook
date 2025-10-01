namespace SharedCookbook.Application.Recipes.Commands.DeleteRecipe;

public record DeleteRecipeCommand(int Id) : IRequest;
public class DeleteRecipeCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteRecipeCommand>
{
    public async Task Handle(DeleteRecipeCommand command, CancellationToken cancellationToken)
    {
        var recipe = await context.Recipes.FindOrThrowAsync(command.Id, cancellationToken);

        context.Recipes.Remove(recipe);

        // TODO add domain event with logging

        await context.SaveChangesAsync(cancellationToken);
    }
}

public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    public DeleteRecipeCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
