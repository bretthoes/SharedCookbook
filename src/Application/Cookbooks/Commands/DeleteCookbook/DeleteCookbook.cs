namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;

public class DeleteCookbookCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteCookbookCommand>
{
    public async Task Handle(DeleteCookbookCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var cookbook = await context.Cookbooks.FindOrThrowAsync(command.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(cookbook.Id, user.Id, ct);

        if (actorMembership is null || !actorMembership.IsOwner)
            throw new ForbiddenAccessException();

        context.Cookbooks.Remove(cookbook);
        cookbook.AddDomainEvent(new CookbookDeletedEvent(cookbook));

        await context.SaveChangesAsync(ct);
    }
}
