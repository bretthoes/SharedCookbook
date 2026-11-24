namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public sealed record DeleteInvitationCommand(Guid Id) : IRequest;

public sealed class DeleteInvitationCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteInvitationCommand>
{
    public async Task Handle(DeleteInvitationCommand command, CancellationToken ct = default)
    {
        var invitation = await context.CookbookInvitations.FindOrThrowAsync(command.Id, ct);

        context.CookbookInvitations.Remove(invitation);

        await context.SaveChangesAsync(ct);
    }
}
