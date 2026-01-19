using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Recipes.Commands.CreateRecipe;
using SharedCookbook.Application.Recipes.Queries.GetRecipe;
using SharedCookbook.Tests.Shared;

namespace SharedCookbook.Application.FunctionalTests.Recipes.Queries;

using static Testing;
using static TestData;
using static RecipeTestData;

public class GetRecipeTests : BaseTestFixture
{
    private RecipeDetailedDto? _actual;
    private int _recipeId;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await ResetState();
        await RunAsDefaultUserAsync();
        
        int cookbookId = await SendAsync(new CreateCookbookCommand(Title: AnyNonEmptyString));

        var command = new CreateRecipeCommand
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
                Ingredients =
                [
                    new RecipeIngredientDto
                    {
                        Name = IngredientName,
                        Ordinal = IngredientOrdinal,
                        Optional = IngredientOptional
                    }
                ],
                Directions =
                [
                    new RecipeDirectionDto
                    {
                        Text = DirectionText,
                        Ordinal = DirectionOrdinal,
                        Image = null
                    }
                ],
                Images =
                [
                    new RecipeImageDto
                    {
                        Name = ImageName,
                        Ordinal = ImageOrdinal
                    }
                ]
            }
        };

        _recipeId = await SendAsync(command);

        _actual = await SendAsync(new GetRecipeQuery(_recipeId));
    }
    
    [Test]
    public void ShouldNotBeNull() => Assert.That(_actual, Is.Not.Null);

    [Test]
    public void ShouldHaveId() => Assert.That(_actual!.Id, Is.EqualTo(_recipeId));
    
    [Test]
    public void ShouldHaveTitle() => Assert.That(_actual!.Title, Is.EqualTo(expected: Title));
    
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
    public void ShouldHaveImageUrlPrefix()
        => Assert.That(_actual!.Images.First().Name, Does.StartWith(expected: "http"));
    
    [Test]
    public void ShouldHaveImageNameSuffix()
        => Assert.That(_actual!.Images.First().Name, Does.EndWith(expected: ImageName));
    
    [Test]
    public void ShouldHaveImageOrdinal()
        => Assert.That(_actual!.Images.First().Ordinal, Is.EqualTo(expected: ImageOrdinal));
}
