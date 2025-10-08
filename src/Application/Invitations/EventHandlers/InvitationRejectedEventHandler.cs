namespace SharedCookbook.Application.Invitations.EventHandlers;

public sealed class InvitationRejectedEventHandler(ILogger<InvitationRejectedEventHandler> logger)
    : INotificationHandler<InvitationRejectedEvent>
{
    public Task Handle(InvitationRejectedEvent rejectedEvent, CancellationToken token)
    {
        var invitation = rejectedEvent.Invitation; 
        
        logger.LogInformation("InvitationRejectedEvent handled: Invitation (ID: {Id}) was rejected for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            invitation.Id,
            invitation.CookbookId,
            invitation.CreatedBy);

        return Task.CompletedTask;
    }
}
