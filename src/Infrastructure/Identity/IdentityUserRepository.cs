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
using SharedCookbook.Infrastructure.Identity.RepositoryExtensions;

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
    public async Task<PaginatedList<MembershipDto>> GetMemberships(
        GetMembershipsWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        return await context.CookbookMemberships
            .AsNoTracking()
            .HasCookbookId(query.CookbookId)
            .Join(
                context.People,
                membership => membership.CreatedBy,
                identityUser => identityUser.Id,
                (membership, identityUser) => new MembershipDto
                {
                    Id = membership.Id,
                    IsCreator = membership.IsCreator,
                    CanAddRecipe = membership.CanAddRecipe,
                    CanUpdateRecipe = membership.CanUpdateRecipe,
                    CanDeleteRecipe = membership.CanDeleteRecipe,
                    CanSendInvite = membership.CanSendInvite,
                    CanRemoveMember = membership.CanRemoveMember,
                    CanEditCookbookDetails = membership.CanEditCookbookDetails,
                    Name = identityUser.DisplayName,
                    Email = identityUser.Email
                })
            .OrderByName()
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }

    public Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken)
        => context.CookbookInvitations
            .AsNoTracking()
            .GetInvitationsForUserByStatus(user.Id, query.Status)
            .Join(
                context.People,
                invitation => invitation.CreatedBy,
                identityUser => identityUser.Id,
                (invitation, identityUser) => new InvitationDto
                {
                    Id = invitation.Id,
                    Created = invitation.Created,
                    CreatedBy = invitation.CreatedBy,
                    CookbookTitle = invitation.Cookbook == null
                        ? ""
                        : invitation.Cookbook.Title,
                    CookbookImage = invitation.Cookbook == null || string.IsNullOrWhiteSpace(invitation.Cookbook.Image)
                        ? ""
                        : $"{options.Value.ImageBaseUrl}{invitation.Cookbook.Image}",
                    SenderName = identityUser.DisplayName ?? "",
                    SenderEmail = identityUser.Email
                })
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
        =>context.Recipes.AsNoTracking()
            .HasCookbookId(query.CookbookId)
            .TitleContains(query.Search)
            .IncludeRecipeDetails()
            .OrderByTitle()
            .Join(context.People,
                recipe => recipe.CreatedBy,
                identityUser => identityUser.Id,
                (recipe, identityUser) => new RecipeDetailedDto
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    Summary = recipe.Summary,
                    AuthorEmail = identityUser.Email,
                    Author = identityUser.DisplayName,
                    Thumbnail = string.IsNullOrWhiteSpace(recipe.Thumbnail)
                        ? ""
                        : $"{options.Value.ImageBaseUrl}{recipe.Thumbnail}",
                    VideoPath = string.IsNullOrWhiteSpace(recipe.VideoPath)
                        ? ""
                        : $"{options.Value.ImageBaseUrl}{recipe.VideoPath}",
                    PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
                    CookingTimeInMinutes = recipe.CookingTimeInMinutes,
                    BakingTimeInMinutes = recipe.BakingTimeInMinutes,
                    Servings = recipe.Servings,
                    Directions = recipe.Directions,
                    Images = recipe.Images.Select(image => new RecipeImage
                    {
                        Id = image.Id,
                        Name = $"{options.Value.ImageBaseUrl}{image.Name}",
                        Ordinal = image.Ordinal,
                    }).ToList(),
                    Ingredients = recipe.Ingredients,
                    IsVegan = recipe.IsVegan,
                    IsVegetarian = recipe.IsVegetarian,
                    IsCheap = recipe.IsCheap,
                    IsHealthy = recipe.IsHealthy,
                    IsDairyFree = recipe.IsDairyFree,
                    IsGlutenFree = recipe.IsGlutenFree,
                    IsLowFodmap = recipe.IsLowFodmap,
                })
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
}

// TODO move this somewhere more appropriate; perform this refactoring for other DTO mappings in this file
public static class CookbookBriefDtoQueries
{
    public static IQueryable<CookbookBriefDto> SelectBriefDto(
        this IQueryable<Cookbook> cookbooks,
        IQueryable<ApplicationUser> people,
        string imageBaseUrl)
        => cookbooks.Join(
                people,
                c => c.CreatedBy,
                p => p.Id,
                (c, p) => new CookbookBriefDto
                {
                    Id           = c.Id,
                    Title        = c.Title,
                    Image        = (c.Image != null && c.Image != "")
                                   ? (imageBaseUrl + c.Image)
                                   : "",
                    MembersCount = c.Memberships.Count,
                    RecipeCount  = c.Recipes.Count,
                    Author       = p.DisplayName,
                    AuthorEmail  = p.Email ?? ""
                });
}
