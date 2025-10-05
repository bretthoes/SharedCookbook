namespace SharedCookbook.Application.Memberships.EventHandlers;

public class PromotedToOwnerEventHandler(IApplicationDbContext context)
    : INotificationHandler<PromotedToOwnerEvent>
{
    public async Task Handle(PromotedToOwnerEvent notification, CancellationToken cancellationToken)
    {
        var membership = notification.Membership; // TODO only pass the id and cookbookId, dont need to pass whole obj. change domain event to a record
        // TODO move to query extensions
        var others = await context.CookbookMemberships
            .Where(otherMembership => otherMembership.CookbookId == membership.CookbookId
                                      && otherMembership.Id != membership.Id && otherMembership.IsOwner)
            .ToListAsync(cancellationToken);

        foreach (var otherMembership in others) otherMembership.Demote();
    }
}
