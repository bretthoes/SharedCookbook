using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class DeleteCookbookTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }
    
    [Test]
    public void ShouldRequireValidCookbookId()
    {
        var command = new DeleteCookbookCommand(Id: 99);

        Assert.That(() => SendAsync(command), Throws.TypeOf<NotFoundException>());
    }

    [Test]
    public async Task ShouldDeleteCookbook()
    {
        var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var cookbook = await FindAsync<Cookbook>(cookbookId);
        Assert.That(cookbook, Is.Not.Null);
        
        await SendAsync(new DeleteCookbookCommand(cookbookId));

        cookbook = await FindAsync<Cookbook>(cookbookId);
        Assert.That(cookbook, Is.Null);
    }
}
