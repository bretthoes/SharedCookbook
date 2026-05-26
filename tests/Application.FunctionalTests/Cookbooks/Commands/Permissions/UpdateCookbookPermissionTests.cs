using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenContributorUpdatesCookbook : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsContributor();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateCookbookCommand(_cookbookId, Title: "Updated Title")),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberUpdatesCookbook : BaseTestFixture
{
    private int _cookbookId;

    [SetUp]
    public async Task SetUp()
    {
        var context = await CreateWithContributor();
        _cookbookId = context.CookbookId;
        await ActAsNonMember();
    }

    [Test]
    public void ShouldThrowForbiddenAccessException() =>
        Assert.That(
            () => SendAsync(new UpdateCookbookCommand(_cookbookId, Title: "Updated Title")),
            Throws.TypeOf<ForbiddenAccessException>());
}
