namespace SharedCookbook.Application.Common.Extensions;

public static class MembershipQueryExtensions
{
    extension(IQueryable<CookbookMembership> query)
    {
        public IQueryable<CookbookMembership> HasCookbookId(int cookbookId) =>
            query.Where(membership => membership.CookbookId == cookbookId);

        public Task<bool> ExistsFor(int cookbookId,
            string userId,
            CancellationToken cancellationToken) 
            => query.HasCookbookId(cookbookId).ForUserId(userId).AsNoTracking().AnyAsync(cancellationToken);

        public IQueryable<CookbookMembership> ForUserId(string userId) =>
            query.Where(membership => membership.CreatedBy == userId);

        public IQueryable<CookbookMembership> OwnersForCookbookExcept(int cookbookId, int exceptMembershipId) =>
            query.AsTracking().HasCookbookId(cookbookId).IsOwner().ExcludingId(exceptMembershipId);

        private IQueryable<CookbookMembership> IsOwner() =>
            query.Where(membership => membership.IsOwner);

        private IQueryable<CookbookMembership> ExcludingId(int id) =>
            query.Where(membership => membership.Id != id);
    }

    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
}
