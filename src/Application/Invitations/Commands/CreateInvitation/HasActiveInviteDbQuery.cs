namespace SharedCookbook.Application.Invitations.Commands.CreateInvitation;

internal static class HasActiveInviteDbQuery
{
    extension(IQueryable<CookbookInvitation> query)
    {
        internal Task<bool> HasActiveInvite(
            Guid cookbookId,
            string recipientPersonId,
            CancellationToken ct = default)
            => query
                .AsNoTracking()
                .AnyAsync(invitation =>
                    invitation.CookbookId == cookbookId
                    && invitation.RecipientPersonId == recipientPersonId
                    && invitation.Status == InvitationStatus.Active, ct);
    }
}
