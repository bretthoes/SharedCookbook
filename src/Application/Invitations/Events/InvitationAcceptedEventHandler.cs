namespace SharedCookbook.Application.Invitations.Events;

public class InvitationAcceptedEventHandler : INotificationHandler<InvitationAcceptedEvent>
{
    private readonly IApplicationDbContext _context;

    public InvitationAcceptedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(InvitationAcceptedEvent acceptedEvent, CancellationToken cancellationToken)
    {
        var newMembership = CookbookMembership.GetDefaultMembership(acceptedEvent.Invitation.CookbookId);
        
        // TODO break this query into smaller extensions
        var membershipAlreadyExists = await _context.CookbookMembers
            .AnyAsync(member => member.CookbookId == newMembership.CookbookId
                                && member.CreatedBy == newMembership.CreatedBy,
                cancellationToken);
        
        if (membershipAlreadyExists) return;
        
        await _context.CookbookMembers.AddAsync(newMembership, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
