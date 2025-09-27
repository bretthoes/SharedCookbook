using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.Projections;

internal static class MembershipProjections
{
    internal static IQueryable<MembershipDto> SelectMembershipDto(
        this IQueryable<CookbookMembership> memberships,
        IQueryable<ApplicationUser> people) =>
        from membership in memberships
        join applicationUser in people on membership.CreatedBy equals applicationUser.Id
        select new MembershipDto
        {
            Id = membership.Id,
            IsCreator = membership.IsCreator,
            CanAddRecipe = membership.CanAddRecipe,
            CanUpdateRecipe = membership.CanUpdateRecipe,
            CanDeleteRecipe = membership.CanDeleteRecipe,
            CanSendInvite = membership.CanSendInvite,
            CanRemoveMember = membership.CanRemoveMember,
            CanEditCookbookDetails = membership.CanEditCookbookDetails,
            Name = applicationUser.DisplayName,
            Email = applicationUser.Email ?? ""
        };
}
