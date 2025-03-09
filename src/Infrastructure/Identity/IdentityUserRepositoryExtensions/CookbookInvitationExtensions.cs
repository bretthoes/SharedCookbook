using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.Identity.IdentityUserRepositoryExtensions;

public static class CookbookInvitationExtensions
{
    public static IQueryable<CookbookInvitation> GetInvitationsForUserByStatus(
        this IQueryable<CookbookInvitation> query,
        string? userId,
        CookbookInvitationStatus status)
        => query
            .Where(invitation
                => invitation.RecipientPersonId == userId
                   && invitation.InvitationStatus == status);

    public static IQueryable<InvitationDto> OrderByMostRecentlyCreated(
        this IQueryable<InvitationDto> invitations)
        => invitations.OrderByDescending(invitation => invitation.Created);
    
    public static InvitationDto MapToJoinedDto(this CookbookInvitation invitation, ApplicationUser user)
        => new InvitationDto
        {
            Id = invitation.Id,
            Created = invitation.Created,
            CreatedBy = invitation.CreatedBy,
            CookbookTitle = invitation.Cookbook == null
                ? ""
                : invitation.Cookbook.Title,
            CookbookImage = invitation.Cookbook == null
                ? ""
                : invitation.Cookbook.Image,
            SenderName = !string.IsNullOrWhiteSpace(user.DisplayName) 
                ? user.DisplayName 
                : user.UserName ?? user.Email,
            SenderEmail = user.Email
        };
}
