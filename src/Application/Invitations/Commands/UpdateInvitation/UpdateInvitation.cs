using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Commands.UpdateInvitation;

public record UpdateInvitationCommand : IRequest<int>
{
    public int Id { get; init; }

    public CookbookInvitationStatus NewStatus { get; init; }
}

public class UpdateInvitationCommandHandler(
    IApplicationDbContext context,
    IUser user,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateInvitationCommand, int>
{
    public async Task<int> Handle(UpdateInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations.FindAsync(keyValues: [command.Id], cancellationToken);
        Guard.Against.NotFound(command.Id, invitation);
        Guard.Against.Null(user.Id);

        // TODO currently violates SRP; we handle updating the invitation, and if accepted, we also handle
        // the resulting action of adding a membership. We are doing this to avoid having writes elsewhere
        // in a domain event handler until outbox pattern or something similar is implemented for more
        // sophisticated transaction handling.
        bool shouldSave = false;
        switch (command.NewStatus)
        {
            case CookbookInvitationStatus.Accepted:
                if (InvitationIsNotAlreadyAccepted(invitation, command.NewStatus))
                {
                    invitation.Accept(timestamp: timeProvider.GetUtcNow().UtcDateTime);
                    await context.CookbookInvitations.AddAsync(invitation, cancellationToken);
                    shouldSave = true;
                }

                if (await UserDoesNotHaveMembershipInCookbook(invitation.CookbookId, user.Id, cancellationToken))
                {
                    var membership = CookbookMembership.GetDefaultMembership(invitation.CookbookId);
                    await context.CookbookMemberships.AddAsync(membership, cancellationToken);
                    shouldSave = true;
                }
                
                break;
            case CookbookInvitationStatus.Unknown:
            case CookbookInvitationStatus.Sent:
            case CookbookInvitationStatus.Rejected:
            default:
                break;
        }

        if (shouldSave) await context.SaveChangesAsync(cancellationToken);
        
        return invitation.Id;
    }

    private static bool InvitationIsNotAlreadyAccepted(
        CookbookInvitation invitation,
        CookbookInvitationStatus newStatus)
        => invitation.IsNotAccepted && newStatus == CookbookInvitationStatus.Accepted;
    
    private async Task<bool> UserDoesNotHaveMembershipInCookbook(
        int cookbookId,
        string userId,
        CancellationToken token) 
        => !await context.CookbookMemberships
            .AnyAsync(membership
                => membership.CookbookId == cookbookId
                   && membership.CreatedBy == userId, token);
}
