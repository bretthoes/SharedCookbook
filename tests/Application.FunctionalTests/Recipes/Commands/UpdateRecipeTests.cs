using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands;

using static Testing;

public class UpdateRecipeTests : BaseTestFixture
{
    [SetUp]
    public async Task SetUp()
    {
        await RunAsDefaultUserAsync();
    }

    [Test]
    public void ShouldRequireValidRecipeId()
    {
        var command = new UpdateRecipeCommand(new UpdateRecipeDto{ Title = "Test Title", Id = 99 });
        Assert.ThrowsAsync<NotFoundException>(() => SendAsync(command));
    }
    
    [Test]
     public async Task ShouldUpdateRecipe() {
         var userId = GetUserId();
    
         var cookbookId = await SendAsync(new CreateCookbookCommand(Title: "New Cookbook"));
   
         var recipeId = await SendAsync(new CreateRecipeCommand
         {
             Recipe = new CreateRecipeDto
             {
                 Title = "New Recipe",
                 CookbookId = cookbookId,
             }
         });
         var command = new UpdateRecipeCommand(new UpdateRecipeDto { Title = "New Recipe Title", Id = recipeId });
    
         await SendAsync(command);
    
         var item = await FindAsync<Recipe>(recipeId);
    
         using (Assert.EnterMultipleScope())
         {
             Assert.That(item, Is.Not.Null);
             Assert.That(item!.Title, Is.EqualTo(command.Recipe.Title));
             Assert.That(item.LastModifiedBy, Is.Not.Null);
             Assert.That(item.LastModifiedBy, Is.EqualTo(userId));
             Assert.That(item.LastModified, Is.EqualTo(DateTimeOffset.Now).Within(TimeSpan.FromSeconds(10)));
         }
     }
}
