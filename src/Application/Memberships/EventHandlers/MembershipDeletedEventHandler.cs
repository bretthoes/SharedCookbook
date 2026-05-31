namespace SharedCookbook.Application.Memberships.EventHandlers;

public class MembershipDeletedEventHandler(
    IUser user,
    INotificationFanOut fanOut,
    ILogger<MembershipDeletedEventHandler> logger)
    : INotificationHandler<MembershipDeletedEvent>
{
    public async Task Handle(MembershipDeletedEvent notification, CancellationToken ct = default)
    {
        var membership = notification.Membership;
        var subjectUserId = membership.CreatedBy;

        if (string.IsNullOrWhiteSpace(subjectUserId))
            return;

        if (membership.CreatedBy == user.Id)
        {
            logger.LogInformation(
                "User {UserId} with membership {MembershipId} has left cookbook {CookbookId}.",
                user.Id,
                membership.Id,
                membership.CookbookId);

            await fanOut.FanOutAsync(
                membership.CookbookId,
                subjectUserId,
                CookbookNotificationActionType.MemberLeft,
                subjectUserId: subjectUserId,
                ct: ct);
        }
        else
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

            logger.LogInformation(
                "User {UserId} with membership {MembershipId} has been removed by User {AdminId} from cookbook {CookbookId}",
                membership.CreatedBy,
                membership.Id,
                user.Id,
                membership.CookbookId);

            await fanOut.FanOutAsync(
                membership.CookbookId,
                user.Id,
                CookbookNotificationActionType.MemberRemoved,
                subjectUserId: subjectUserId,
                ct: ct);
        }
    }
}
