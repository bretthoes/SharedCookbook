using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class DeleteCookbookTests : BaseTestFixture
{
    public async Task ShouldRequireValidCookbookId()
    {
        var command = new DeleteCookbookCommand(Id: 99);
        await FluentActions.Invoking((() => SendAsync(command))).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteCookbook()
    {
        await RunAsDefaultUserAsync();

        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var cookbook = await FindAsync<Cookbook>(cookbookId);
        cookbook.Should().NotBeNull();
        
        await SendAsync(new DeleteCookbookCommand(cookbookId));

        cookbook = await FindAsync<Cookbook>(cookbookId);
        cookbook.Should().BeNull();
    }
}
