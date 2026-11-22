using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.InvitationTokens.Commands.CreateInvitationToken;
using SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;
using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Application.FunctionalTests.InvitationTokens.Queries;

using static Testing;

public class WhenCookbookCoverIsUploaded : BaseTestFixture
{
    private const string ImageFile = "cover.webp";
    private const string OwnerEmail = "owner-cover@test.local";
    private const string GuestEmail = "guest-cover@test.local";
    private const string Password = "Testing1234!";

    [Test]
    public async Task ShareLinkPreviewIncludesImageUrl()
    {
        await RunAsDefaultUserAsync();
        var cookbookId = await CreateCookbookWithCover();
        var link = await SendAsync(new CreateInvitationTokenCommand(cookbookId));

        var preview = await SendAsync(new GetInvitationTokenQuery(link.Token));

        Assert.That(preview.CookbookImage, Is.EqualTo(ExpectedImageUrl()));
    }

    [Test]
    public async Task PendingInvitationIncludesImageUrl()
    {
        await RunAsUserAsync(OwnerEmail, Password, []);
        var cookbookId = await CreateCookbookWithCover();
        await RunAsUserAsync(GuestEmail, Password, []);
        await RunAsExistingUserAsync(OwnerEmail);
        await SendAsync(new CreateInvitationCommand(cookbookId, GuestEmail));
        await RunAsExistingUserAsync(GuestEmail);

        var page = await SendAsync(new GetInvitationsWithPaginationQuery());

        Assert.That(page.Items.Single().CookbookImage, Is.EqualTo(ExpectedImageUrl()));
    }

    private static Task<Guid> CreateCookbookWithCover() =>
        SendAsync(new CreateCookbookCommand(Title: "Family Recipes", Image: ExpectedImageUrl()));

    private static string ExpectedImageUrl() => GetImageBaseUrl() + ImageFile;
}
