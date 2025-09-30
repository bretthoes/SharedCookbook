namespace SharedCookbook.Application.Common.Extensions;

public static class InvitationTokenQueryExtensions
{
    public static Task<InvitationToken> SingleById(
        this IQueryable<InvitationToken> query,
        Guid tokenId,
        CancellationToken cancellationToken = default) =>
        query
            .AsNoTracking()
            .SingleAsync(token => token.PublicId == tokenId, cancellationToken);
}
