namespace SharedCookbook.Application.Memberships.Queries.GetDepartingOwners;

internal static class GetDepartingOwnersDbQuery
{
    extension(IQueryable<CookbookMembership> query)
    {
        internal Task<List<CookbookMembership>> GetDepartingOwners(
            int cookbookId,
            int promotedMembershipId,
            CancellationToken ct = default) =>
            query.HasCookbookId(cookbookId)
                .Where(member => member.Tier == MembershipTier.Owner && member.Id != promotedMembershipId)
                .ToListAsync(ct);
    }
}
