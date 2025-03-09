using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.RepositoryExtensions;

public static class CookbookMemberExtensions
{
    public static IQueryable<CookbookMember> HasCookbookId(
        this IQueryable<CookbookMember> query,
        int cookbookId)
        => query.Where(member => member.CookbookId == cookbookId);
    
    public static IQueryable<MembershipDto> OrderByName(
        this IQueryable<MembershipDto> query)
        => query.OrderByDescending(dto => dto.Name);
}
