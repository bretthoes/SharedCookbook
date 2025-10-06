namespace SharedCookbook.Application.Invitations.EventHandlers;

public class InvitationAcceptedEventHandler(
    IApplicationDbContext context,
    IUser user, // TODO maybe move this out, relies on http context in event handler
    ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        if (await context.CookbookMemberships.ExistsFor(acceptedEvent.CookbookId, user.Id, token)) return;

        var membership = CookbookMembership.NewDefault(acceptedEvent.CookbookId, user.Id);
        await context.CookbookMemberships.AddAsync(membership, token);
        
        logger.LogInformation(
            "InvitationAcceptedEvent handled: Invitation (ID: {InvitationId}) was accepted for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            acceptedEvent.InvitationId,
            acceptedEvent.CookbookId,
            user.Id);
    }
}
