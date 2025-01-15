using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class CreateCookbookTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateCookbookCommand
        {
            Title = null!
        };

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateCookbook()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateCookbookCommand
        {
            Title = "Another New Cookbook",
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<Cookbook>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    }
}
