namespace SharedCookbook.Application.Memberships.EventHandlers;

public class MembershipUpdatedEventHandler(IUser user, ILogger<MembershipUpdatedEventHandler> logger)
    : INotificationHandler<MembershipUpdatedEvent>
{
    public Task Handle(MembershipUpdatedEvent notification, CancellationToken ct = default)
    {
        var membership = notification.Membership;

        logger.LogInformation(
            "User {UserId} membership {MembershipId} tier set to {Tier} by user {AdminId} in cookbook {CookbookId}",
            membership.CreatedBy,
            membership.Id,
            membership.Tier,
            user.Id,
            membership.CookbookId);

        return Task.CompletedTask;
    }
}
