using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Common;

using static Testing;
using static RecipeTestData;

internal sealed record CookbookWithContributorContext(
    int CookbookId,
    string OwnerUserId,
    string ContributorUserId,
    int RecipeId,
    int OwnerMembershipId,
    int ContributorMembershipId);

internal static class CookbookPermissionScenario
{
    internal const string ContributorPassword = "Testing1234!";
    internal const string ContributorEmail = "contributor@test.local";
    internal const string NonMemberEmail = "outsider@test.local";
    internal const string InviteeEmail = "invitee@test.local";
    internal const string OwnerEmail = "test@local";

    internal static async Task<CookbookWithContributorContext> CreateWithContributor(
        MembershipTier contributorTier = MembershipTier.Contributor)
    {
        var contributorUserId = await RunAsUserAsync(ContributorEmail, ContributorPassword, []);
        var ownerUserId = await RunAsDefaultUserAsync();
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: TestData.AnyNonEmptyString));
        var recipeId = await SendAsync(new CreateRecipeCommand { Recipe = GetSimpleCreateRecipeDto(cookbookId) });

        var membership = CookbookMembership.NewDefault(cookbookId, contributorUserId);
        membership.SetTier(contributorTier);

        await AddAsync(membership);

        var contributorMembershipId = (await SingleAsync<CookbookMembership>(membership =>
            membership.CookbookId == cookbookId && membership.CreatedBy == contributorUserId)).Id;
        var ownerMembershipId = (await SingleAsync<CookbookMembership>(membership =>
            membership.CookbookId == cookbookId && membership.CreatedBy == ownerUserId)).Id;

        return new CookbookWithContributorContext(
            cookbookId,
            ownerUserId,
            contributorUserId,
            recipeId,
            ownerMembershipId,
            contributorMembershipId);
    }

    internal static Task<string> ActAsContributor() =>
        RunAsExistingUserAsync(ContributorEmail);

    internal static Task<string> ActAsOwner() =>
        RunAsExistingUserAsync(OwnerEmail);

    internal static Task<string> ActAsNonMember() =>
        RunAsUserAsync(NonMemberEmail, ContributorPassword, []);

    internal static Task<string> EnsureInviteeExists() =>
        RunAsUserAsync(InviteeEmail, ContributorPassword, []);
}
