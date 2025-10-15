using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsCount;

public sealed record GetInvitationsCountQuery(InvitationStatus Status = InvitationStatus.Active)
    : IRequest<int>;

public sealed class GetInvitationsCountQueryHandler(IIdentityUserRepository repository)
    : IRequestHandler<GetInvitationsCountQuery, int>
{
    public Task<int> Handle(GetInvitationsCountQuery query, CancellationToken token)
        => repository.GetInvitationsCount(query, token);
}
