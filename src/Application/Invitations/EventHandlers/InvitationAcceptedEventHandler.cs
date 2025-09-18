namespace SharedCookbook.Application.Invitations.EventHandlers;

public class InvitationAcceptedEventHandler(ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken token)
    {
        var invitation = acceptedEvent.Invitation; 
        
        logger.LogInformation("InvitationAcceptedEvent handled: Invitation (ID: {Id}) was accepted for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            invitation.Id,
            invitation.CookbookId,
            invitation.CreatedBy);

        return Task.CompletedTask;
    }
}
