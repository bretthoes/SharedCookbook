namespace SharedCookbook.Application.Memberships.Commands.PatchMembership;

public sealed record PatchMembershipCommand : IRequest<int>
{
    public int Id { get; init; }

}

public sealed class PatchMembershipCommandHandler : IRequestHandler<PatchMembershipCommand, int>
{
    public Task<int> Handle(PatchMembershipCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
