using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;

internal static class GetMembershipsDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<PaginatedList<MembershipDto>> QueryDtos(
            Guid cookbookId,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => query
                .HasCookbookId(cookbookId)
                .Select(membership => new MembershipDto
                {
                    Id   = membership.Id,
                    Tier = membership.Tier,
                    Name = membership.DisplayName,
                })
                .OrderByDescending(dto => dto.Name)
                .PaginatedListAsync(pageNumber, pageSize, ct);
    }
}
