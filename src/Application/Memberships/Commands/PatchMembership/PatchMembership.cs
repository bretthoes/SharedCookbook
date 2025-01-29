namespace SharedCookbook.Application.Memberships.Commands.PatchMembership;

public record PatchMembershipCommand : IRequest<int>
{
    public int Id { get; init; }

}

public class PatchMembershipCommandHandler : IRequestHandler<PatchMembershipCommand, int>
{
    private readonly IApplicationDbContext _context;

    public PatchMembershipCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> Handle(PatchMembershipCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
