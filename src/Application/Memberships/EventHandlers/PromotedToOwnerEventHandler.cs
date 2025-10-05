namespace SharedCookbook.Application.Memberships.EventHandlers;

public class PromotedToOwnerEventHandler(IApplicationDbContext context)
    : INotificationHandler<PromotedToOwnerEvent>
{
    public async Task Handle(PromotedToOwnerEvent notification, CancellationToken cancellationToken)
    {
        var others = await context.CookbookMemberships
            .OwnersForCookbookExcept(notification.CookbookId, notification.MembershipId)
            .ToListAsync(cancellationToken);
        
        foreach (var otherMembership in others) otherMembership.Demote();
    }
}
