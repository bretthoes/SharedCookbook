using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.IdentityUserRepositoryExtensions;

public static class CookbookMemberExtensions
{
    public static IQueryable<CookbookMember> HasCookbookId(
        this IQueryable<CookbookMember> query,
        int cookbookId)
        => query.Where(member => member.CookbookId == cookbookId);
    
    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);

    public static MembershipDto MapToJoinedDto(this CookbookMember member, ApplicationUser user)
        => new MembershipDto
        {
            Id = member.Id,
            IsCreator = member.IsCreator,
            CanAddRecipe = member.CanAddRecipe,
            CanUpdateRecipe = member.CanUpdateRecipe,
            CanDeleteRecipe = member.CanDeleteRecipe,
            CanSendInvite = member.CanSendInvite,
            CanRemoveMember = member.CanRemoveMember,
            CanEditCookbookDetails = member.CanEditCookbookDetails,
            Name = user.UserName,
            Email = user.Email
        };
}
