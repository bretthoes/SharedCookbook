using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Users.Commands.DeleteAccount;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;
using SharedCookbook.Infrastructure.Identity;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Users.Commands;

using static Testing;

public class DeleteAccountTests : BaseTestFixture
{
    private const string OwnerEmail = "owner@test.local";
    private const string MemberEmail = "member@test.local";
    private const string Password = "Testing1234!";

    [Test]
    public void ShouldRequireAuthenticatedUser()
    {
        Assert.That(() => SendAsync(new DeleteAccountCommand()), Throws.TypeOf<UnauthorizedAccessException>());
    }

    [Test]
    public async Task ShouldDeletePrivateCookbook()
    {
        await RunAsUserAsync(OwnerEmail, Password, []);
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "Solo"));

        await SendAsync(new DeleteAccountCommand());

        Assert.That(await FindAsync<Cookbook>(cookbookId), Is.Null);
    }

    [Test]
    public async Task ShouldDeleteUser()
    {
        var userId = await RunAsUserAsync(OwnerEmail, Password, []);

        await SendAsync(new DeleteAccountCommand());

        Assert.That(await FindAsync<ApplicationUser>(userId), Is.Null);
    }

    [Test]
    public async Task ShouldKeepSharedCookbook()
    {
        var cookbookId = await CreateSharedCookbookAsync();

        await SendAsync(new DeleteAccountCommand());

        Assert.That(await FindAsync<Cookbook>(cookbookId), Is.Not.Null);
    }

    [Test]
    public async Task ShouldPromoteRemainingMemberToOwner()
    {
        await CreateSharedCookbookAsync();
        var memberId = await RunAsExistingUserAsync(MemberEmail);
        await RunAsExistingUserAsync(OwnerEmail);

        await SendAsync(new DeleteAccountCommand());

        var membership = await SingleAsync<CookbookMembership>(membership => membership.CreatedBy == memberId);
        Assert.That(membership.Tier, Is.EqualTo(MembershipTier.Owner));
    }

    [Test]
    public async Task ShouldDeleteShareLinksTheyCreated()
    {
        var cookbookId = await CreateSharedCookbookAsync();
        await SendAsync(new CreateInvitationTokenCommand(cookbookId));

        await SendAsync(new DeleteAccountCommand());

        Assert.That(await AnyAsync<InvitationToken>(token => token.CookbookId == cookbookId), Is.False);
    }

    [Test]
    public async Task ShouldClearRecipeAuthorOnSharedCookbook()
    {
        var recipeId = await CreateSharedRecipeAsync();

        await SendAsync(new DeleteAccountCommand());

        var recipe = await FindAsync<Recipe>(recipeId);
        Assert.That(recipe!.CreatedBy, Is.Null);
    }

    private static async Task<Guid> CreateSharedCookbookAsync()
    {
        await RunAsUserAsync(OwnerEmail, Password, []);
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "Shared"));
        var memberId = await RunAsUserAsync(MemberEmail, Password, []);
        await AddAsync(CookbookMembership.NewDefault(cookbookId, memberId, "Member"));
        await RunAsExistingUserAsync(OwnerEmail);
        return cookbookId;
    }

    private static async Task<Guid> CreateSharedRecipeAsync()
    {
        var cookbookId = await CreateSharedCookbookAsync();
        return await SendAsync(new CreateRecipeCommand
        {
            Recipe = RecipeTestData.GetSimpleCreateRecipeDto(cookbookId),
        });
    }
}
