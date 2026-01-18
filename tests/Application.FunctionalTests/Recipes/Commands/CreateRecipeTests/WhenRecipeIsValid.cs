using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.CreateRecipeTests;

using static Testing;

public class WhenRecipeIsValid : BaseTestFixture
{
    [Test]
    public async Task ShouldCreateRecipe()
    {
        string? userId = GetUserId();

        int cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));

        var command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = "Recipe Title",
                CookbookId = cookbookId
            }
        };

        var itemId = await SendAsync(command);

        var item = await FindAsync<Recipe>(itemId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(item, Is.Not.Null);
            Assert.That(item!.Title, Is.EqualTo(command.Recipe.Title));
            Assert.That(item.CreatedBy, Is.EqualTo(userId));
            Assert.That(item.Created, Is.EqualTo(DateTimeOffset.Now).Within(TimeSpan.FromSeconds(1)));
            Assert.That(item.LastModifiedBy, Is.EqualTo(userId));
            Assert.That(item.LastModified, Is.EqualTo(DateTimeOffset.Now).Within(TimeSpan.FromSeconds(1)));
        }
    }
}
