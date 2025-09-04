using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
/// Repository for handling queries that involve the AspNetUsers table,
/// which is managed by the Identity framework. This repository is defined
/// in the Infrastructure layer (and accessed through an interface in the
/// Application layer) to avoid directly referencing the IdentityUser in
/// the Application project. 
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
            .QueryCookbooksForMember(context, user.Id)
            .Join(context.People,
                cookbook => cookbook.CreatedBy,
                identityUser => identityUser.Id,
                (cookbook, identityUser) => new CookbookBriefDto
                {
                    Id = cookbook.Id,
                    Title = cookbook.Title,
                    Image = string.IsNullOrWhiteSpace(cookbook.Image)
                        ? ""
                        : $"{options.Value.ImageBaseUrl}{cookbook.Image}",
                    MembersCount = cookbook.Memberships.Count,
                    RecipeCount = cookbook.Recipes.Count,
                    Author = identityUser.DisplayName,
                    AuthorEmail = identityUser.Email ?? ""
                })
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
