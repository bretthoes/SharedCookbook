namespace SharedCookbook.Application.Memberships.EventHandlers;

public class PromotedToOwnerEventHandler(IApplicationDbContext context)
    : INotificationHandler<PromotedToOwnerEvent>
{
    public async Task Handle(PromotedToOwnerEvent notification, CancellationToken ct = default)
    {
        var others = await context.CookbookMemberships
            .OwnersForCookbookExcept(notification.CookbookId, notification.MembershipId)
            .ToListAsync(ct);
        
        foreach (var otherMembership in others) otherMembership.Demote();
    }
}
