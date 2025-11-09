namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public sealed record DeleteInvitationCommand(int Id) : IRequest;

public sealed class DeleteInvitationCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteInvitationCommand>
{
    public async Task Handle(DeleteInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations.FindOrThrowAsync(command.Id, cancellationToken);

        context.CookbookInvitations.Remove(invitation);

        invitation.AddDomainEvent(new InvitationDeletedEvent(invitation));

        await context.SaveChangesAsync(cancellationToken);
    }
}
