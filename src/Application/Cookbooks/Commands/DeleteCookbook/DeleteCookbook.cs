namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;

public class DeleteCookbookCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteCookbookCommand>
{
    public async Task Handle(DeleteCookbookCommand command, CancellationToken ct = default)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(command.Id, ct);

        if (!await CanDeleteCookbook(cookbook.Id, ct))
            throw new ForbiddenAccessException();

        context.Cookbooks.Remove(cookbook);
        cookbook.AddDomainEvent(new CookbookDeletedEvent(cookbook));

        await context.SaveChangesAsync(ct);
    }

    private async Task<bool> CanDeleteCookbook(int cookbookId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return false;

        var actor = await context.CookbookMemberships.FindForUserAsync(cookbookId, user.Id, ct);

        return actor is not null && actor.IsOwner;
    }
}
