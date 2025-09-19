using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.RepositoryExtensions;

// TODO should move these to application
public static class CookbookMemberExtensions
{
    public static IQueryable<CookbookMembership> HasCookbookId(
        this IQueryable<CookbookMembership> query,
        int cookbookId)
        => query.Where(member => member.CookbookId == cookbookId);
    
    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
}
