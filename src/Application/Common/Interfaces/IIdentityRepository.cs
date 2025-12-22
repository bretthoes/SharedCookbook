using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IIdentityRepository
{
    Task<PaginatedList<MembershipDto>> GetMemberships(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken);

    Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken);

    Task<PaginatedList<CookbookBriefDto>> GetCookbooks(
        GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken);
}
