using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Data;
using SharedCookbook.Infrastructure.FileStorage;
using SharedCookbook.Infrastructure.Identity.Projections;

namespace SharedCookbook.Infrastructure.Identity;

/// <summary>
/// Repository for handling queries with joins to the AspNetUsers table,
/// which is managed by the Identity framework. This repository is defined
/// in the Infrastructure layer (and injected through an interface in the
/// Application layer) to avoid directly referencing the IdentityUser in
/// the Application project. The tradeoff here is that knowledge of
/// Identity is kept out of the application layer, but the downside is
/// the necessity of this repository and the verbose joining/mapping that
/// must be defined in the query.
/// </summary>
/// <remarks>
/// This repository should be limited to read-only operations, as modifying
/// AspNetUsers should be handled exclusively by Identity services provided
/// by ASP.NET.
/// </remarks>
public class IdentityUserRepository(ApplicationDbContext context, IUser user, IOptions<ImageUploadOptions> options)
    : IIdentityUserRepository
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

    public Task<PaginatedList<RecipeDetailedDto>> GetRecipes(
        GetRecipesWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.Recipes.AsNoTracking()
            .HasCookbookId(query.CookbookId)
            .TitleContains(query.Search)
            .IncludeRecipeDetails()
            .OrderByTitle()
            .SelectRecipeDetailedDto(context.People.AsNoTracking(), options.Value.ImageBaseUrl)
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
}
