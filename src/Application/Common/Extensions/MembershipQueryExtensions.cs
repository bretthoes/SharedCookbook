namespace SharedCookbook.Application.Common.Extensions;

public static class MembershipQueryExtensions
{
    extension(IQueryable<CookbookMembership> query)
    {
        public IQueryable<CookbookMembership> HasCookbookId(int cookbookId) =>
            query.Where(membership => membership.CookbookId == cookbookId);

        public Task<bool> ExistsFor(int cookbookId,
            string userId,
            CancellationToken ct = default) 
            => query.HasCookbookId(cookbookId).ForUserId(userId).AsNoTracking().AnyAsync(ct);

        private IQueryable<CookbookMembership> ForUserId(string userId) =>
            query.Where(membership => membership.CreatedBy == userId);

        public Task<CookbookMembership> GetByCookbookAndUser(int cookbookId, string userId, CancellationToken ct = default) =>
            query.FindForUserOrThrowAsync(cookbookId, userId, ct);

        public Task<CookbookMembership?> FindForUserAsync(int cookbookId, string userId, CancellationToken ct = default) =>
            query.ForCookbookAndUser(cookbookId, userId).SingleOrDefaultAsync(ct);

        public async Task<CookbookMembership> FindForUserOrThrowAsync(
            int cookbookId,
            string userId,
            CancellationToken ct = default) =>
            await query.FindForUserAsync(cookbookId, userId, ct) ??
            throw new NotFoundException(key: $"{cookbookId}:{userId}", nameof(CookbookMembership));

        private IQueryable<CookbookMembership> ForCookbookAndUser(int cookbookId, string userId) =>
            query.HasCookbookId(cookbookId).ForUserId(userId);
    }

    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
}
