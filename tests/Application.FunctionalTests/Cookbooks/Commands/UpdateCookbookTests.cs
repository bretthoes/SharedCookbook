using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class UpdateCookbookTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public void ShouldRequireValidCookbookId()
    {
        var command = new UpdateCookbookCommand(Id: 99, Title: "New Title");

        Assert.That(() => SendAsync(command), Throws.TypeOf<NotFoundException>());
    }

    [Test, Ignore("Title doesn't have to be unique, just keeping test around as an example for now")]
    public async Task ShouldRequireUniqueTitle()
    {
        await RunAsDefaultUserAsync();

        var cookbookId = await SendAsync(new CreateCookbookCommand("New Cookbook"));
        await SendAsync(new CreateCookbookCommand("Other Cookbook"));

        var command = new UpdateCookbookCommand(cookbookId, "Other List");

        var ex = Assert.ThrowsAsync<ValidationException>(() => SendAsync(command));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.Errors.ContainsKey("Title"), Is.True);
            Assert.That(ex.Errors["Title"], Does.Contain("'Title must be unique."));
        }
    }

    [Test]
    public async Task ShouldUpdateCookbook()
    {
        string? userId = GetUserId();

        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var command = new UpdateCookbookCommand(Id: cookbookId, Title: "Updated Cookbook Title");

        await SendAsync(command);

        var cookbook = await FindAsync<Cookbook>(cookbookId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(cookbook, Is.Not.Null);
            Assert.That(cookbook!.Title, Is.EqualTo(command.Title));
            Assert.That(cookbook.LastModifiedBy, Is.Not.Null);
            Assert.That(cookbook.LastModifiedBy, Is.EqualTo(userId));
            Assert.That(cookbook.LastModified, Is.EqualTo(DateTimeOffset.Now).Within(1).Seconds);
        }
    }
}
