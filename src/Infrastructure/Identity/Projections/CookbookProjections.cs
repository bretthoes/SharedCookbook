using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Identity.Projections;

internal static class CookbookProjections
{
    public static IQueryable<CookbookBriefDto> SelectBriefDto(
        this IQueryable<Cookbook> cookbooks,
        IQueryable<ApplicationUser> people,
        string imageBaseUrl)
        => cookbooks.Join(
            people,
            cookbook => cookbook.CreatedBy,
            applicationUser => applicationUser.Id,
            (cookbook, applicationUser) => new CookbookBriefDto
            {
                Id           = cookbook.Id,
                Title        = cookbook.Title,
                Image        = cookbook.Image!.EnsurePrefixUrl(imageBaseUrl),
                MembersCount = cookbook.Memberships.Count,
                RecipeCount  = cookbook.Recipes.Count,
                Author       = applicationUser.DisplayName,
                AuthorEmail  = applicationUser.Email ?? ""
            });
}
