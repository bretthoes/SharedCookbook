using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

public sealed record GetInvitationsWithPaginationQuery(
    InvitationStatus Status = InvitationStatus.Active,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedList<InvitationDto>>;

public sealed class GetInvitationsWithPaginationQueryHandler(IIdentityRepository repository)
    : IRequestHandler<GetInvitationsWithPaginationQuery, PaginatedList<InvitationDto>>
{
    public Task<PaginatedList<InvitationDto>> Handle(GetInvitationsWithPaginationQuery query, CancellationToken token)
        => repository.GetInvitations(query, token);
}
