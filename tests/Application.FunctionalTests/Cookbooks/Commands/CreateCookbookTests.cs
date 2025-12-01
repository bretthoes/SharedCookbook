using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class CreateCookbookTests : BaseTestFixture
{
    
    [TestCase(null!)]
    [TestCase("")]
    public async Task ShouldRequireMinimumFields(string title)
    {
        var command = new CreateCookbookCommand(Title: title);

        await FluentActions.Invoking((() => SendAsync(command))).Should().ThrowAsync<ValidationException>();
    }

    [TestCase(null!)]
    [TestCase("image")]
    public async Task ShouldCreateCookbook(string image)
    {
        string? userId = GetUserId();

        var command = new CreateCookbookCommand(Title: "New Cookbook", Image: image);

        int itemId = await SendAsync(command);

        var item = await FindAsync<Cookbook>(itemId);

        item.Should().NotBeNull();
        item.Title.Should().Be(command.Title);
        item.Image.Should().Be(command.Image);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
    }

    [Test]
    public async Task ShouldCreateOwnerMembership()
    {
        string? userId = GetUserId();
        var command = new CreateCookbookCommand(Title: "New Cookbook");

        int cookbookId = await SendAsync(command);

        var membership = await FindAsync<CookbookMembership>(cookbookId);

        membership.Should().NotBeNull();
        membership.CookbookId.Should().Be(cookbookId);
        membership.CreatedBy.Should().Be(userId);
        membership.Created.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
        membership.LastModifiedBy.Should().Be(userId);
        membership.LastModified.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
    }
}
