using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Cookbooks.Commands;

using static Testing;

public class CreateCookbookTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }
    
    [TestCase(null!)]
    [TestCase("")]
    public void ShouldRequireMinimumFields(string title)
    {
        var command = new CreateCookbookCommand(Title: title);

        Assert.That(() => SendAsync(command), Throws.TypeOf<ValidationException>());
    }

    [TestCase(null!)]
    [TestCase("image")]
    public async Task ShouldCreateCookbook(string image)
    {
        string? userId = GetUserId();

        var command = new CreateCookbookCommand(Title: "New Cookbook", Image: image);

        int itemId = await SendAsync(command);

        var item = await FindAsync<Cookbook>(itemId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item!.Title, Is.EqualTo(command.Title));
            Assert.That(item.Image, Is.EqualTo(command.Image));
            Assert.That(item.CreatedBy, Is.EqualTo(userId));
            Assert.That(item.LastModifiedBy, Is.EqualTo(userId));
            Assert.That(item.Created, Is.EqualTo(DateTimeOffset.Now).Within(1).Seconds);
            Assert.That(item.LastModified, Is.EqualTo(DateTimeOffset.Now).Within(1).Seconds);
        }
    }

    [Test]
    public async Task ShouldCreateOwnerMembership()
    {
        string? userId = GetUserId();
        var command = new CreateCookbookCommand(Title: "New Cookbook");

        int cookbookId = await SendAsync(command);

        var membership = await FindAsync<CookbookMembership>(cookbookId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(membership, Is.Not.Null);
            Assert.That(membership!.CookbookId, Is.EqualTo(cookbookId));
            Assert.That(membership.CreatedBy, Is.EqualTo(userId));
            Assert.That(membership.LastModifiedBy, Is.EqualTo(userId));
            Assert.That(membership.Created, Is.EqualTo(DateTimeOffset.Now).Within(1).Seconds);
            Assert.That(membership.LastModified, Is.EqualTo(DateTimeOffset.Now).Within(1).Seconds);
        }
    }
}
