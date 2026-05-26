using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands.Permissions;

using static Testing;
using static Common.CookbookPermissionScenario;

public class WhenContributorDeletesCookbook : BaseTestFixture
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
            () => SendAsync(new DeleteCookbookCommand(_cookbookId)),
            Throws.TypeOf<ForbiddenAccessException>());
}

public class WhenNonMemberDeletesCookbook : BaseTestFixture
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
            () => SendAsync(new DeleteCookbookCommand(_cookbookId)),
            Throws.TypeOf<ForbiddenAccessException>());
}
