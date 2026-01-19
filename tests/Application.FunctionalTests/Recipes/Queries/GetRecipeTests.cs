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
    public void ShouldHaveTitle() => Assert.That(_actual!.Title, Is.EqualTo(Title));
    
    [Test]
    public void ShouldHaveSummary() => Assert.That(_actual!.Summary, Is.EqualTo(Summary));
    
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

    [Test]
    public void ShouldHaveIngredients()
        => Assert.That(_actual!.Ingredients, Has.Count.EqualTo(1));
    
    [Test]
    public void ShouldHaveIngredientName()
        => Assert.That(_actual!.Ingredients.First().Name, Is.EqualTo(IngredientName));
    
    [Test]
    public void ShouldHaveIngredientOrdinal()
        => Assert.That(_actual!.Ingredients.First().Ordinal, Is.EqualTo(IngredientOrdinal));
    
    [Test]
    public void ShouldHaveIngredientOptional()
        => Assert.That(_actual!.Ingredients.First().Optional, Is.EqualTo(IngredientOptional));

    [Test]
    public void ShouldHaveDirections()
        => Assert.That(_actual!.Directions, Has.Count.EqualTo(1));
    
    [Test]
    public void ShouldHaveDirectionText()
        => Assert.That(_actual!.Directions.First().Text, Is.EqualTo(DirectionText));
    
    [Test]
    public void ShouldHaveDirectionOrdinal()
        => Assert.That(_actual!.Directions.First().Ordinal, Is.EqualTo(DirectionOrdinal));

    [Test]
    public void ShouldHaveImages()
        => Assert.That(_actual!.Images, Has.Count.EqualTo(1));
    
    [Test]
    public void ShouldHaveImageName()
        => Assert.That(_actual!.Images.First().Name, Is.EqualTo(ImageName));
    
    [Test]
    public void ShouldHaveImageOrdinal()
        => Assert.That(_actual!.Images.First().Ordinal, Is.EqualTo(ImageOrdinal));
}
