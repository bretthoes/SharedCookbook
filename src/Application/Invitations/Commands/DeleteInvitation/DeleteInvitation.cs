namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public record DeleteInvitationCommand(int Id) : IRequest;
public class DeleteInvitationCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteInvitationCommand>
{
    public async Task Handle(DeleteInvitationCommand request, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations
            .FindAsync(keyValues: [request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, invitation);

        context.CookbookInvitations.Remove(invitation);

        invitation.AddDomainEvent(new InvitationDeletedEvent(invitation));

        await context.SaveChangesAsync(cancellationToken);
    }
}
