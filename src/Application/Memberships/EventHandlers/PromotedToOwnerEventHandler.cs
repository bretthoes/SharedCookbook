namespace SharedCookbook.Application.Memberships.EventHandlers;

public class PromotedToOwnerEventHandler(ILogger<PromotedToOwnerEventHandler> logger)
    : INotificationHandler<PromotedToOwnerEvent>
{
    public Task Handle(PromotedToOwnerEvent notification, CancellationToken ct = default)
    {
        logger.LogInformation(
            "PromotedToOwnerEvent handled: Membership {MembershipId} was promoted to owner in cookbook {CookbookId}.",
            notification.MembershipId,
            notification.CookbookId);

        return Task.CompletedTask;
    }
}
