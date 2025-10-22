namespace SharedCookbook.Application.Memberships.EventHandlers;

public class MembershipDeletedEventHandler(IUser user, ILogger<MembershipDeletedEventHandler> logger)
    : INotificationHandler<MembershipDeletedEvent>
{
    public Task Handle(MembershipDeletedEvent notification, CancellationToken cancellationToken)
    {
        var membership = notification.Membership;
        
        if (membership.CreatedBy == user.Id)
        {
            logger.LogInformation(
                "User {UserId} with membership {MembershipId} has left cookbook {CookbookId}.",
                user.Id,
                membership.Id,
                membership.CookbookId);
        }
        else
        {
            logger.LogInformation(
                "User {UserId} with membership {MembershipId} has been removed by User {AdminId} from cookbook {CookbookId}",
                membership.CreatedBy,
                membership.Id,
                user.Id,
                membership.CookbookId);
        }

        return Task.CompletedTask;
    }
}
