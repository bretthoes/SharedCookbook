namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;

public class DeleteCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteCookbookCommand>
{
    public async Task Handle(DeleteCookbookCommand command, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(command.Id, cancellationToken);

        context.Cookbooks.Remove(cookbook);

        // TODO add domain event for logging

        await context.SaveChangesAsync(cancellationToken);
    }
}
public class DeleteCookbookCommandValidator : AbstractValidator<DeleteCookbookCommand>
{
    public DeleteCookbookCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
