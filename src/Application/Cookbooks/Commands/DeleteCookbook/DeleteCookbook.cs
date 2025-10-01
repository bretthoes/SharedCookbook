namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;

public class DeleteCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteCookbookCommand>
{
    public async Task Handle(DeleteCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks
            .FindAsync(keyValues: [request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, cookbook);

        context.Cookbooks.Remove(cookbook);

        //entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

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
