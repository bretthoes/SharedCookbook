using Microsoft.EntityFrameworkCore;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;
using SharedCookbook.Infrastructure.Data;

namespace SharedCookbook.Infrastructure;

/// <summary>
/// Repository for handling queries that involve the AspNetUsers table,
/// which is managed by the Identity framework. This repository is defined
/// in the Infrastructure layer (and accessed through an interface in the
/// Application layer) to avoid directly referencing the IdentityUser in
/// the Application project. 
///
/// Note:
/// This repository should be limited to read-only operations, as modifying
/// AspNetUsers should be handled exclusively by Identity services provided
/// by ASP.NET.
/// </summary>
public class IdentityUserRepository : IIdentityUserRepository
{
    private readonly ApplicationDbContext _context;

    public IdentityUserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<MembershipDto>> GetMembershipsWithUserDetailsAsync(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await _context.CookbookMembers
            .Where(member => member.CookbookId == query.CookbookId)
            .Join(
                _context.People,
                member => member.CreatedBy,
                user => user.Id,
                (member, user) => new MembershipDto
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
                })
            .AsNoTracking()
            .OrderByDescending(c => c.Name)
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }
}
