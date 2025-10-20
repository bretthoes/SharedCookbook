namespace SharedCookbook.Application.Memberships.EventHandlers;

public class MembershipUpdatedEventHandler(IUser user, ILogger<MembershipUpdatedEventHandler> logger)
    : INotificationHandler<MembershipUpdatedEvent>
{
    public Task Handle(MembershipUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var membership = notification.Membership;
        
            logger.LogInformation(
                "User {UserId} with membership {MembershipId} has been updated by User {AdminId} in cookbook {CookbookId}",
                membership.CreatedBy,
                membership.Id,
                user.Id,
                membership.CookbookId);

        return Task.CompletedTask;
    }
}
