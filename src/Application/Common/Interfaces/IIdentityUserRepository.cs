using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Application.Common.Interfaces;
public interface IIdentityUserRepository
{
    Task<PaginatedList<MembershipDto>> GetMemberships(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken);

    Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken);
}
