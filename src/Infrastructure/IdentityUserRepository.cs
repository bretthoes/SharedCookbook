using Microsoft.EntityFrameworkCore;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
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
    private readonly IUser _user;

    public IdentityUserRepository(ApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<MembershipDto>> GetMemberships(
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

    public Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        return _context.CookbookInvitations
            .AsNoTracking()
            .Where(invitation => invitation.RecipientPersonId == _user.Id 
                && invitation.InvitationStatus == query.Status)
            .Join(
                _context.People,
                invitation => invitation.CreatedBy,
                user => user.Id,
                (invitation, user) => new InvitationDto
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
                    SenderName = user.UserName,
                    SenderEmail = user.Email
                })
            .OrderByDescending(c => c.Created)
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }
}
