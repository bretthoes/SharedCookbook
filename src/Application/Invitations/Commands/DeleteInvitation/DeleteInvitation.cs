using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public record DeleteInvitationCommand(int Id) : IRequest;
public class DeleteInvitationCommandHandler : IRequestHandler<DeleteInvitationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteInvitationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CookbookInvitations
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.CookbookInvitations.Remove(entity);

        entity.AddDomainEvent(new InvitationDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
