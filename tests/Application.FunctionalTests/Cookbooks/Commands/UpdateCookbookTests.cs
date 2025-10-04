using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class UpdateCookbookTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidCookbookId()
    {
        var command = new UpdateCookbookCommand(Id: 99, Title: "New Title");
        await FluentActions.Invoking((() => SendAsync(command))).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await RunAsDefaultUserAsync();

        var cookbookId = await SendAsync(new CreateCookbookCommand("New Cookbook"));
        await SendAsync(new CreateCookbookCommand("Other Cookbook"));
        var command = new UpdateCookbookCommand(cookbookId, "Other List");
        (await FluentActions.Invoking((() =>
                    SendAsync(command)))
                .Should().ThrowAsync<ValidationException>().Where(exception => exception.Errors.ContainsKey("Title")))
            .And.Errors["Title"].Should().Contain("'Title must be unique.");
    }

    [Test]
    public async Task ShouldUpdateCookbook()
    {
        var userId = await RunAsDefaultUserAsync();

        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var command = new UpdateCookbookCommand(Id: cookbookId, Title: "Updated Cookbook Title");

        await SendAsync(command);

        var cookbook = await FindAsync<Cookbook>(cookbookId);

        cookbook.Should().NotBeNull();
        cookbook!.Title.Should().Be(command.Title);
        cookbook.LastModifiedBy.Should().NotBeNull();
        cookbook.LastModifiedBy.Should().Be(userId);
        cookbook.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
