using SharedCookbook.Application.Memberships.Queries;

namespace SharedCookbook.Application.Common.Extensions;

public static class CookbookMemberExtensions
{
    
    public static IQueryable<CookbookMembership> HasCookbookId(
        this IQueryable<CookbookMembership> query,
        int cookbookId)
        => query.Where(membership => membership.CookbookId == cookbookId);
    
    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
}
