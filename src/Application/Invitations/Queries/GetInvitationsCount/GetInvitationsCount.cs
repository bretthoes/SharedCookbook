using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsCount;

public sealed record GetInvitationsCountQuery(InvitationStatus Status = InvitationStatus.Active)
    : IRequest<int>;

public sealed class GetInvitationsCountQueryHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<GetInvitationsCountQuery, int>
{
    public Task<int> Handle(GetInvitationsCountQuery query, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        return context.CookbookInvitations.GetCountByUserAndStatus(user.Id, query.Status, token);
    }
}
