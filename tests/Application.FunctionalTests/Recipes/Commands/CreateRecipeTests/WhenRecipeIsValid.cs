using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.CreateRecipeTests;

using static Testing;

public class WhenRecipeIsValid : BaseTestFixture
{
    private const string RecipeTitle = "Recipe Title";
    private const string RecipeSummary = "Recipe Summary";
    private const int RecipeServings = 1;
    private const int PreparationTimeInMinutes = 2;
    private const int CookingTimeInMinutes = 3;
    private const int BakingTimeInMinutes = 4;
    
    private CreateRecipeCommand _command = null!;
    private Recipe? _actual;
    private string _userId = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        _userId = await RunAsDefaultUserAsync();
        
        int cookbookId = await SendAsync(new CreateCookbookCommand(Title: TestData.AnyNonEmptyString));

        _command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = RecipeTitle,
                CookbookId = cookbookId,
                Summary = RecipeSummary,
                PreparationTimeInMinutes = PreparationTimeInMinutes,
                CookingTimeInMinutes = CookingTimeInMinutes,
                BakingTimeInMinutes = BakingTimeInMinutes,
                Servings = RecipeServings,
            }
        };

        int recipeId = await SendAsync(_command);

        _actual = await FindAsync<Recipe>([recipeId]);
    }
    
    [Test]
    public void ShouldNotBeNull() => Assert.That(_actual, Is.Not.Null);

    [Test]
    public void ShouldHaveCreated() =>
        Assert.That(_actual!.Created, Is.EqualTo(DateTimeOffset.Now).Within(TimeSpan.FromSeconds(1)));
    
    [Test]
    public void ShouldHaveCreatedBy() => Assert.That(_actual!.CreatedBy, Is.EqualTo(_userId));
    
    [Test]
    public void ShouldHaveLastModified() =>
        Assert.That(_actual!.LastModified,
            Is.EqualTo(DateTimeOffset.Now).Within(TimeSpan.FromSeconds(1)));
    
    [Test]
    public void ShouldHaveLastModifiedBy() =>
        Assert.That(_actual!.LastModifiedBy, Is.EqualTo(_userId));
    
    [Test]
    public void ShouldHaveTitle() => Assert.That(_actual!.Title, Is.EqualTo(expected: RecipeTitle));
    
    [Test]
    public void ShouldHaveServings() => Assert.That(_actual!.Servings, Is.EqualTo(expected: RecipeServings));
    
    [Test]
    public void ShouldHavePreparationTimeInMinutes()
        => Assert.That(_actual!.PreparationTimeInMinutes, Is.EqualTo(expected: PreparationTimeInMinutes));
    
    [Test]
    public void ShouldHaveCookingTimeInMinutes()
        => Assert.That(_actual!.CookingTimeInMinutes, Is.EqualTo(expected: CookingTimeInMinutes));
    
    [Test]
    public void ShouldHaveBakingTimeInMinutes()
        => Assert.That(_actual!.BakingTimeInMinutes, Is.EqualTo(expected: BakingTimeInMinutes));
}
