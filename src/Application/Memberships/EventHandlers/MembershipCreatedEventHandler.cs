namespace SharedCookbook.Application.Memberships.EventHandlers;

public sealed class MembershipCreatedEventHandler(INotificationFanOut fanOut)
    : INotificationHandler<MembershipCreatedEvent>
{
    public Task Handle(MembershipCreatedEvent notification, CancellationToken ct = default)
    {
        var membership = notification.Membership;

        if (membership.IsOwner || string.IsNullOrWhiteSpace(membership.CreatedBy))
            return Task.CompletedTask;

        return fanOut.FanOutAsync(
            membership.CookbookId,
            membership.CreatedBy,
            CookbookNotificationActionType.MemberJoined,
            subjectUserId: membership.CreatedBy,
            ct: ct);
    }
}
