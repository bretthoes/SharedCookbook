using SharedCookbook.Application.Memberships.Commands.CreateMembership;

namespace SharedCookbook.Application.Invitations.Events;

public class InvitationAcceptedEventHandler(IApplicationDbContext context, IUser user, IMediator mediator)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken token)
    {
        Guard.Against.Null(user.Id);
        
        if (await MembershipAlreadyExistsInCookbook(acceptedEvent.Invitation.CookbookId, user.Id, token)) return;

        var command = new CreateMembershipCommand { CookbookId = acceptedEvent.Invitation.CookbookId };

        await mediator.Send(command, token);
    }

    private async Task<bool> MembershipAlreadyExistsInCookbook(
        int cookbookId,
        string userId,
        CancellationToken token) 
        => await context.CookbookMemberships
            .AnyAsync(membership
                => membership.CookbookId == cookbookId
                   && membership.CreatedBy == userId, token);
}
