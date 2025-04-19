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
        var membership = await _context.CookbookMemberships
            .FindAsync(keyValues: [request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, membership);

        _context.CookbookMemberships.Remove(membership);

        //entity.AddDomainEvent(new MembershipDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
