namespace SharedCookbook.Application.Invitations.Events;

public class InvitationAcceptedEventHandler(IApplicationDbContext context)
    : INotificationHandler<InvitationAcceptedEvent>
{
    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken cancellationToken)
    {
        var candidateMembership = CookbookMembership.GetDefaultMembership(acceptedEvent.Invitation.CookbookId);
        
        bool alreadyExists = await context.CookbookMemberships
            .AnyAsync(membership => MatchesMembership(membership, candidateMembership), cancellationToken);

        
        if (alreadyExists) return;
        
        await context.CookbookMemberships.AddAsync(candidateMembership, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    private static bool MatchesMembership(CookbookMembership membership, CookbookMembership candidate)
        => membership.CookbookId == candidate.CookbookId
           && membership.CreatedBy == candidate.CreatedBy;

}
