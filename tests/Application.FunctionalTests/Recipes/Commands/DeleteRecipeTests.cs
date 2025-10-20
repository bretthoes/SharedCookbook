using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Commands.DeleteRecipe;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands;

using static Testing;

public class DeleteRecipeTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidRecipeId()
    {
        var command = new DeleteRecipeCommand(Id: 99);

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteRecipe()
    {
        var cookbook = new Cookbook { Title = "Test Cookbook Title" };
        await AddAsync(cookbook);
        
        var itemId = await SendAsync(new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = "Another Recipe Title",
                CookbookId = cookbook.Id,
                Directions = new List<RecipeDirectionDto>
                {
                    new()
                    {
                        Text = "Test Direction",
                        Ordinal = 0
                    }
                },
                Ingredients = new List<CreateRecipeIngredientDto>
                {
                    new()
                    {
                        Name = "Test Ingredient",
                        Optional = false,
                        Ordinal = 0
                    }
                }
            }
        });

        await SendAsync(new DeleteRecipeCommand(itemId));

        var item = await FindAsync<Recipe>(itemId);

        item.Should().BeNull();
    }
}
