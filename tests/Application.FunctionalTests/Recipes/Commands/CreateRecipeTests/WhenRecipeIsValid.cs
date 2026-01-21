using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.CreateRecipeTests;

using static Testing;
using static TestData;
using static RecipeTestData;

public class WhenRecipeIsValid : BaseTestFixture
{
    private CreateRecipeCommand _command = null!;
    private Recipe? _actual;
    private string _userId = null!;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        _userId = await RunAsDefaultUserAsync();
        
        int cookbookId = await SendAsync(new CreateCookbookCommand(Title: AnyNonEmptyString));

        _command = new CreateRecipeCommand
        {
            Recipe = new CreateRecipeDto
            {
                Title = Title,
                CookbookId = cookbookId,
                Summary = Summary,
                PreparationTimeInMinutes = PreparationTimeInMinutes,
                CookingTimeInMinutes = CookingTimeInMinutes,
                BakingTimeInMinutes = BakingTimeInMinutes,
                Servings = Servings,
            }
        };

        int recipeId = await SendAsync(_command);

        _actual = await FindAsync<Recipe>([recipeId]);
    }
    
    [Test]
    public void ShouldNotBeNull() => Assert.That(_actual, Is.Not.Null);
    
    [Test]
    public void ShouldHaveId() => Assert.That(_actual!.Id, Is.GreaterThan(expected: 0));
    
    [Test]
    public void ShouldHaveCookbookId() => Assert.That(_actual!.CookbookId, Is.GreaterThan(expected: 0));

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
    public void ShouldHaveTitle() => Assert.That(_actual!.Title, Is.EqualTo(expected: Title));
    
    [Test]
    public void ShouldHaveServings() => Assert.That(_actual!.Servings, Is.EqualTo(Servings));
    
    [Test]
    public void ShouldHavePreparationTimeInMinutes()
        => Assert.That(_actual!.PreparationTimeInMinutes, Is.EqualTo(PreparationTimeInMinutes));
    
    [Test]
    public void ShouldHaveCookingTimeInMinutes()
        => Assert.That(_actual!.CookingTimeInMinutes, Is.EqualTo(CookingTimeInMinutes));
    
    [Test]
    public void ShouldHaveBakingTimeInMinutes()
        => Assert.That(_actual!.BakingTimeInMinutes, Is.EqualTo(BakingTimeInMinutes));
}
