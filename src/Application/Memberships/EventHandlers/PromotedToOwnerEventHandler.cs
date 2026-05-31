namespace SharedCookbook.Application.Memberships.EventHandlers;

using SharedCookbook.Application.Memberships.Queries.GetDepartingOwners;

public class PromotedToOwnerEventHandler(
    IApplicationDbContext context,
    ILogger<PromotedToOwnerEventHandler> logger)
    : INotificationHandler<PromotedToOwnerEvent>
{
    public async Task Handle(PromotedToOwnerEvent notification, CancellationToken ct = default)
    {
        var departingOwners = await context.CookbookMemberships.GetDepartingOwners(
            notification.CookbookId,
            notification.MembershipId,
            ct);

        foreach (var owner in departingOwners)
            owner.Demote();

        logger.LogInformation(
            "PromotedToOwnerEvent handled: Membership {MembershipId} was promoted to owner in cookbook {CookbookId}.",
            notification.MembershipId,
            notification.CookbookId);
    }
}
