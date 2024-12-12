using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;
public class DeleteMembershipCommandHandler : IRequestHandler<DeleteMembershipCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteMembershipCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteMembershipCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.CookbookMembers
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.CookbookMembers.Remove(entity);

        //entity.AddDomainEvent(new MembershipDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
