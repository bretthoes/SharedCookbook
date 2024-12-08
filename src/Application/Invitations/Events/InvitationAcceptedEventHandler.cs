using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

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
        var newMembership = CookbookMember.GetDefaultMembership(acceptedEvent.Invitation.CookbookId);
        await _context.CookbookMembers.AddAsync(newMembership, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
