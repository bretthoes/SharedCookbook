using SharedCookbook.Application.Memberships.Commands.CreateMembership;

namespace SharedCookbook.Application.Invitations.Events;

public class InvitationAcceptedEventHandler(IMediator mediator, ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken token)
    {
        var invitation = acceptedEvent.Invitation; 
        
        var command = new CreateMembershipCommand { CookbookId = invitation.CookbookId };

        logger.LogInformation("InvitationAcceptedEvent handled: Invitation (ID: {Id}) was accepted for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            invitation.Id,
            invitation.CookbookId,
            invitation.CreatedBy);
        
        await mediator.Send(command, token);
    }
}
