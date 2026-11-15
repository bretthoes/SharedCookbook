using Microsoft.AspNetCore.Identity;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Application.FunctionalTests.Users;

using static Testing;

internal static class ExternalAccountLinkerTestData
{
    internal const string Email = "linked@test.local";
    internal const string Password = "Testing1234!";
    internal const string Provider = "Google";
    internal const string Subject = "google-subject-1";

    internal static Task<Result<string>> LinkAsync(bool isEmailVerified) =>
        UseServiceAsync<ExternalAccountLinker, Result<string>>(linker =>
            linker.FindOrCreateUserAsync(Provider, Subject, Email, isEmailVerified));

    internal static Task<ApplicationUser?> FindByIdAsync(string userId) =>
        UseServiceAsync<UserManager<ApplicationUser>, ApplicationUser?>(manager => manager.FindByIdAsync(userId));

    internal static Task<bool> HasPasswordAsync(string userId) =>
        UseServiceAsync<UserManager<ApplicationUser>, bool>(async manager =>
            await manager.HasPasswordAsync((await manager.FindByIdAsync(userId))!));

    internal static Task<ApplicationUser?> FindByLoginAsync() =>
        UseServiceAsync<UserManager<ApplicationUser>, ApplicationUser?>(manager =>
            manager.FindByLoginAsync(Provider, Subject));
}

public class WhenVerifiedLoginMatchesUnconfirmedAccount : BaseTestFixture
{
    private string _existingUserId = null!;
    private Result<string> _result = null!;
    private bool _hasPassword;
    private bool _isEmailConfirmed;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        _existingUserId = await RunAsUserAsync(ExternalAccountLinkerTestData.Email, ExternalAccountLinkerTestData.Password, []);
        _result = await ExternalAccountLinkerTestData.LinkAsync(isEmailVerified: true);
        _hasPassword = await ExternalAccountLinkerTestData.HasPasswordAsync(_existingUserId);
        _isEmailConfirmed = (await ExternalAccountLinkerTestData.FindByIdAsync(_existingUserId))!.EmailConfirmed;
    }

    [Test]
    public void ReturnsExistingUser() => Assert.That(_result.Value, Is.EqualTo(_existingUserId));

    [Test]
    public void RemovesPasswordSetBeforeConfirmation() => Assert.That(_hasPassword, Is.False);

    [Test]
    public void ConfirmsEmail() => Assert.That(_isEmailConfirmed, Is.True);
}

public class WhenUnverifiedLoginMatchesExistingAccount : BaseTestFixture
{
    private Result<string> _result = null!;
    private ApplicationUser? _linkedUser;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        await RunAsUserAsync(ExternalAccountLinkerTestData.Email, ExternalAccountLinkerTestData.Password, []);
        _result = await ExternalAccountLinkerTestData.LinkAsync(isEmailVerified: false);
        _linkedUser = await ExternalAccountLinkerTestData.FindByLoginAsync();
    }

    [Test]
    public void Fails() => Assert.That(_result.Succeeded, Is.False);

    [Test]
    public void DoesNotLinkLogin() => Assert.That(_linkedUser, Is.Null);
}

public class WhenUnverifiedLoginHasNoAccount : BaseTestFixture
{
    private Result<string> _result = null!;
    private ApplicationUser? _linkedUser;
    private bool _hasPassword;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        _result = await ExternalAccountLinkerTestData.LinkAsync(isEmailVerified: false);
        _linkedUser = await ExternalAccountLinkerTestData.FindByLoginAsync();
        _hasPassword = await ExternalAccountLinkerTestData.HasPasswordAsync(_result.Value!);
    }

    [Test]
    public void Succeeds() => Assert.That(_result.Succeeded, Is.True);

    [Test]
    public void CreatesLinkedUser() => Assert.That(_linkedUser?.Id, Is.EqualTo(_result.Value));

    [Test]
    public void CreatesUserWithoutPassword() => Assert.That(_hasPassword, Is.False);
}

public class WhenEmailHasRfcSpecialCharacters : BaseTestFixture
{
    private const string EmailWithApostrophe = "o'brien+cook@test.local";
    private IdentityResult _result = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        _result = await UseServiceAsync<UserManager<ApplicationUser>, IdentityResult>(manager =>
            manager.CreateAsync(
                new ApplicationUser { UserName = EmailWithApostrophe, Email = EmailWithApostrophe },
                ExternalAccountLinkerTestData.Password));
    }

    [Test]
    public void CreatesUser() => Assert.That(_result.Succeeded, Is.True);
}
