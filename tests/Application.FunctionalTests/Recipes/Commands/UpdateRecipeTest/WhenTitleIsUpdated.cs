using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Recipes.Commands.UpdateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Commands.UpdateRecipeTest;

using static Testing;
using static RecipeTestData;

public class WhenTitleIsUpdated : BaseTestFixture
{
    private RecipeDetailedDto? _actual;
    const string UpdatedTitle = "UpdatedTitle";
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        await RunAsDefaultUserAsync();
        
        int recipeId = await CreateSimpleRecipe();

        var updateDto = GetSimpleUpdateRecipeDto(recipeId, UpdatedTitle);
        
        await SendAsync(new UpdateRecipeCommand(updateDto));
        
        _actual = await SendAsync(new GetRecipeQuery(recipeId));
    }
    
    [Test]
    public void ShouldNotBeNull() => Assert.That(_actual, Is.Not.Null);

    [Test]
    public void ShouldHaveId() => Assert.That(_actual!.Id, Is.GreaterThan(expected: 0));
    
    [Test]
    public void ShouldHaveNewTitle() => Assert.That(_actual!.Title, Is.EqualTo(expected: UpdatedTitle ));
    
    [Test]
    public void ShouldHaveSummary() => Assert.That(_actual!.Summary, Is.EqualTo(expected: Summary));
    
    [Test]
    public void ShouldHaveServings() => Assert.That(_actual!.Servings, Is.EqualTo(expected: Servings));
    
    [Test]
    public void ShouldHavePreparationTimeInMinutes()
        => Assert.That(_actual!.PreparationTimeInMinutes, Is.EqualTo(expected: PreparationTimeInMinutes));
    
    [Test]
    public void ShouldHaveCookingTimeInMinutes()
        => Assert.That(_actual!.CookingTimeInMinutes, Is.EqualTo(expected: CookingTimeInMinutes));
    
    [Test]
    public void ShouldHaveBakingTimeInMinutes()
        => Assert.That(_actual!.BakingTimeInMinutes, Is.EqualTo(BakingTimeInMinutes));

    [Test]
    public void ShouldHaveIngredients()
        => Assert.That(_actual!.Ingredients, Has.Count.EqualTo(expected: 1));
    
    [Test]
    public void ShouldHaveIngredientName()
        => Assert.That(_actual!.Ingredients.First().Name, Is.EqualTo(expected: IngredientName));
    
    [Test]
    public void ShouldHaveIngredientOrdinal()
        => Assert.That(_actual!.Ingredients.First().Ordinal, Is.EqualTo(expected: IngredientOrdinal));
    
    [Test]
    public void ShouldHaveIngredientOptional()
        => Assert.That(_actual!.Ingredients.First().Optional, Is.EqualTo(expected: IngredientOptional));

    [Test]
    public void ShouldHaveDirections()
        => Assert.That(_actual!.Directions, Has.Count.EqualTo(expected: 1));
    
    [Test]
    public void ShouldHaveDirectionText()
        => Assert.That(_actual!.Directions.First().Text, Is.EqualTo(expected: DirectionText));
    
    [Test]
    public void ShouldHaveDirectionOrdinal()
        => Assert.That(_actual!.Directions.First().Ordinal, Is.EqualTo(expected: DirectionOrdinal));

    [Test]
    public void ShouldHaveImages()
        => Assert.That(_actual!.Images, Has.Count.EqualTo(expected: 1));

    [Test]
    public void ShouldHaveFullImageName()
    {
        var expected = ImageName.EnsurePrefixUrl(GetImageBaseUrl());
        
        Assert.That(_actual!.Images.First().Name, Is.EqualTo(expected));
    }
    
    [Test]
    public void ShouldHaveImageOrdinal()
        => Assert.That(_actual!.Images.First().Ordinal, Is.EqualTo(expected: ImageOrdinal));
}
