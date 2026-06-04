namespace SharedCookbook.Application.Invitations.EventHandlers;

public class InvitationAcceptedEventHandler(
    IApplicationDbContext context,
    IIdentityService identityService,
    ILogger<InvitationAcceptedEventHandler> logger)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken ct = default)
    {
        if (await context.CookbookMemberships.ExistsFor(acceptedEvent.CookbookId, acceptedEvent.UserId, ct))
            return;

        var displayName = await identityService.GetDisplayNameAsync(acceptedEvent.UserId, ct) ?? acceptedEvent.UserId;

        var membership = CookbookMembership.NewDefault(acceptedEvent.CookbookId, acceptedEvent.UserId,  displayName);
        membership.AddDomainEvent(new MembershipCreatedEvent(membership));
        await context.CookbookMemberships.AddAsync(membership, ct);
        
        logger.LogInformation(
            "InvitationAcceptedEvent handled: Invitation (ID: {InvitationId}) was accepted for Cookbook (ID: {CookbookId}) by User ID {UserId}.",
            acceptedEvent.InvitationId,
            acceptedEvent.CookbookId,
            acceptedEvent.UserId);
    }
}
