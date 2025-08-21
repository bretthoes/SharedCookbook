using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

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

        await FluentActions.Invoking((() =>
            SendAsync(command))).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateCookbook()
    {
        string userId = await RunAsDefaultUserAsync();

        var command = new CreateCookbookCommand
        {
            Title = "Another New Cookbook",
        };

        int itemId = await SendAsync(command);

        var item = await FindAsync<Cookbook>(itemId);

        item.Should().NotBeNull();
        item!.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().BeCloseTo(DateTime.Now, precision: TimeSpan.FromSeconds(10));
    }
}
