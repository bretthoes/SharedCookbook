namespace SharedCookbook.Application.Invitations.EventHandlers;

public class InvitationAcceptedEventHandler(
    IApplicationDbContext context,
    ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken token)
    {
        if (await context.CookbookMemberships.ExistsFor(acceptedEvent.CookbookId, acceptedEvent.UserId, token)) return;

        var membership = CookbookMembership.NewDefault(acceptedEvent.CookbookId, acceptedEvent.UserId);
        await context.CookbookMemberships.AddAsync(membership, token);
        
        logger.LogInformation(
            "InvitationAcceptedEvent handled: Invitation (ID: {InvitationId}) was accepted for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            acceptedEvent.InvitationId,
            acceptedEvent.CookbookId,
            acceptedEvent.UserId);
    }
}
