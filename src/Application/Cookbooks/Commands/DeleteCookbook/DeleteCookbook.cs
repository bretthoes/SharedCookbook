namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;

public class DeleteCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteCookbookCommand>
{
    public async Task Handle(DeleteCookbookCommand command, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(command.Id, cancellationToken);

        context.Cookbooks.Remove(cookbook);

        cookbook.AddDomainEvent(new CookbookDeletedEvent(cookbook));

        await context.SaveChangesAsync(cancellationToken);
    }
}
