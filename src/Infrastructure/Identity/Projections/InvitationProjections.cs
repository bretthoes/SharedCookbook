using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.Projections;

internal static class InvitationProjections
{
    internal static IQueryable<InvitationDto> SelectInvitationDto(
        this IQueryable<CookbookInvitation> invitations,
        IQueryable<ApplicationUser> people,
        string imageBaseUrl) =>
        from invitation in invitations
        join applicationUser in people on invitation.CreatedBy equals applicationUser.Id
        select new InvitationDto
        {
            Id = invitation.Id,
            Created = invitation.Created,
            CookbookId =  invitation.CookbookId,
            CookbookTitle = invitation.Cookbook != null ? invitation.Cookbook.Title : "",
            CookbookImage = (invitation.Cookbook != null && invitation.Cookbook.Image != null && invitation.Cookbook.Image != "")
                            ? imageBaseUrl + invitation.Cookbook.Image
                            : "",
            SenderName = applicationUser.DisplayName ?? "",
            SenderEmail = applicationUser.Email
        };
}
