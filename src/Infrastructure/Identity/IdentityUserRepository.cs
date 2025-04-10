using Microsoft.EntityFrameworkCore;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Cookbooks.Queries.GetCookbooksWithPagination;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Application.Memberships.Queries;
using SharedCookbook.Application.Memberships.Queries.GetMembershipsWithPagination;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipesWithPagination;
using SharedCookbook.Infrastructure.Data;
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
            .AsNoTracking()
            .HasCookbookId(query.CookbookId)
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
                    Name = user.DisplayName,
                    Email = user.Email
                })
            .OrderByName()
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    }

    public Task<PaginatedList<InvitationDto>> GetInvitations(
        GetInvitationsWithPaginationQuery query,
        CancellationToken cancellationToken)
        => _context.CookbookInvitations
            .AsNoTracking()
            .GetInvitationsForUserByStatus(_user.Id, query.Status)
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
                    SenderName = user.DisplayName ?? "",
                    SenderEmail = user.Email
                })
            .OrderByMostRecentlyCreated()
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);
    
    public Task<PaginatedList<CookbookBriefDto>> GetCookbooks(
        GetCookbooksWithPaginationQuery query,
        CancellationToken cancellationToken)
        => _context.Cookbooks
            .QueryCookbooksForMember(_context, _user.Id)
            .Join(_context.People,
                cookbook => cookbook.CreatedBy,
                user => user.Id,
                (cookbook, user) => new CookbookBriefDto
                {
                    Id = cookbook.Id,
                    Title = cookbook.Title,
                    Image = cookbook.Image,
                    MembersCount = cookbook.CookbookMembers.Count,
                    RecipeCount = cookbook.Recipes.Count,
                    Author = user.DisplayName,
                    AuthorEmail = user.Email ?? ""
                })
            .PaginatedListAsync(query.PageNumber, query.PageSize, cancellationToken);

    public Task<PaginatedList<RecipeDetailedDto>> GetRecipes(
        GetRecipesWithPaginationQuery query,
        CancellationToken cancellationToken)
        =>_context.Recipes.AsNoTracking()
            .HasCookbookId(query.CookbookId)
            .TitleContains(query.Search)
            .IncludeRecipeDetails()
            .OrderByTitle()
            .Join(_context.People,
                recipe => recipe.CreatedBy,
                user => user.Id,
                (recipe, user) => new RecipeDetailedDto
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    Summary = recipe.Summary,
                    AuthorEmail = user.Email,
                    Author = user.DisplayName,
                    Thumbnail = recipe.Thumbnail,
                    VideoPath = recipe.VideoPath,
                    PreparationTimeInMinutes = recipe.PreparationTimeInMinutes,
                    CookingTimeInMinutes = recipe.CookingTimeInMinutes,
                    BakingTimeInMinutes = recipe.BakingTimeInMinutes,
                    Servings = recipe.Servings,
                    Directions = recipe.Directions,
                    Images = recipe.Images,
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
