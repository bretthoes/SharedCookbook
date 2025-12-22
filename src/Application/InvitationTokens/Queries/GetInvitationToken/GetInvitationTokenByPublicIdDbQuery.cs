namespace SharedCookbook.Application.Common.Extensions;

public static class GetInvitationTokenByPublicIdDbQuery
{
    public static Task<InvitationToken?> SingleById(
        this IQueryable<InvitationToken> query, Guid tokenId, CancellationToken cancellationToken = default) =>
        query.Include(token => token.Cookbook)
        .SingleOrDefaultAsync(token => token.PublicId == tokenId, cancellationToken);
}
