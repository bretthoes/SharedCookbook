using SharedCookbook.Application.Common.Mappings;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

internal static class GetInvitationsDbQuery
{
    extension(IQueryable<CookbookInvitation> query)
    {
        internal Task<PaginatedList<InvitationDto>> QueryDtos(
            string userId,
            InvitationStatus status,
            string imageBaseUrl,
            int pageNumber,
            int pageSize,
            CancellationToken ct = default)
            => query
                .GetInvitationsForUserByStatus(userId, status)
                .Select(invitation => new InvitationDto
                {
                    Id            = invitation.Id,
                    Created       = invitation.Created,
                    CookbookId    = invitation.CookbookId,
                    CookbookTitle = invitation.Cookbook != null ? invitation.Cookbook.Title : "",
                    CookbookImage = invitation.Cookbook != null && invitation.Cookbook.Image != null && invitation.Cookbook.Image != "" // TODO simplify this nonsense with a helper
                                        ? imageBaseUrl + invitation.Cookbook.Image
                                        : "",
                    SenderName    = invitation.SenderDisplayName,
                })
                .OrderByDescending(invitation => invitation.Created)
                .PaginatedListAsync(pageNumber, pageSize, ct);
        
        private IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(string? userId, InvitationStatus status)
            => query.Where(invitation => invitation.RecipientPersonId == userId && invitation.Status == status);
        
        // TODO move to another class, since this is a separate query
        public Task<bool> HasActiveInvite(
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
