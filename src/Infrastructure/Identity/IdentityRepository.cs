using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Infrastructure.Identity.Projections;

namespace SharedCookbook.Infrastructure.Identity;

/// <summary>
/// Repository for handling queries with joins to the AspNetUsers table,
/// which is managed by the Identity framework. This repository is implemented
/// in the Infrastructure layer to avoid directly referencing the IdentityUser in
/// the Application layer. The tradeoff here is that knowledge of
/// Identity is kept out of the application layer, but the downside is
/// the necessity of this repository and the verbose joining that
/// must be defined in the query.
/// </summary>
/// <remarks>
/// This repository should be limited to read-only operations, as modifying
/// AspNetUsers should be handled exclusively by Identity services provided
/// by ASP.NET.
/// </remarks>
public class IdentityRepository(ApplicationDbContext context, IUser user, IOptions<ImageUploadOptions> options)
    : IIdentityRepository
{
    public Task<PaginatedList<MembershipDto>> GetMemberships(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.CookbookMemberships
            .AsNoTracking()
            .HasCookbookId(query.CookbookId)
            .SelectMembershipDto(context.People.AsNoTracking())
            .OrderByName()
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

    public Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.CookbookInvitations
            .AsNoTracking()
            .GetInvitationsForUserByStatus(user.Id, query.Status)
            .SelectInvitationDto(context.People.AsNoTracking(), options.Value.ImageBaseUrl)
            .OrderByMostRecentlyCreated()
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

    public Task<PaginatedList<CookbookBriefDto>> GetCookbooks(
        GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.Cookbooks
            .AsNoTracking()
            .ForMember(user.Id)
            .OrderByTitle()
            .SelectBriefDto(context.People.AsNoTracking(), options.Value.ImageBaseUrl)
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
}
