using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

namespace SharedCookbook.Application.Common.Interfaces;
public interface IIdentityUserRepository
{
    Task<PaginatedList<MembershipDto>> GetMembershipsWithUserDetailsAsync(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken);
}
