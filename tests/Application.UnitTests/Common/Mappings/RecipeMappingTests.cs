using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.UnitTests.Common.Mappings;

public class RecipeMappingTests
{
    private const int ExpectedId = 1;
    private const string ExpectedTitle = "TestTitle";
    private const string ExpectedSummary = "TestSummary";
    private const string ExpectedThumbnail = "TestThumbnail";
    private const string ExpectedVideoPath = "TestVideoPath";
    private const int ExpectedPreparationTime = 10;
    private const int ExpectedCookingTime = 20;
    private const int ExpectedBakingTime = 30;
    private const int ExpectedServings = 4;
    private const bool ExpectedIsVegan = true;
    private const bool ExpectedIsVegetarian = true;
    private const bool ExpectedIsCheap = true;
    private const bool ExpectedIsHealthy = true;
    private const bool ExpectedIsDairyFree = true;
    private const bool ExpectedIsGlutenFree = true;
    private const bool ExpectedIsLowFodmap = true;
    private const string ImageBaseUrl = "https://example.com/images/";

    private RecipeDetailedDto _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var sut = new Recipe
        {
            Id = ExpectedId,
            Title = ExpectedTitle,
            Summary = ExpectedSummary,
            Thumbnail = ExpectedThumbnail,
            VideoPath = ExpectedVideoPath,
            PreparationTimeInMinutes = ExpectedPreparationTime,
            CookingTimeInMinutes = ExpectedCookingTime,
            BakingTimeInMinutes = ExpectedBakingTime,
            Servings = ExpectedServings,
            IsVegan = ExpectedIsVegan,
            IsVegetarian = ExpectedIsVegetarian,
            IsCheap = ExpectedIsCheap,
            IsHealthy = ExpectedIsHealthy,
            IsDairyFree = ExpectedIsDairyFree,
            IsGlutenFree = ExpectedIsGlutenFree,
            IsLowFodmap = ExpectedIsLowFodmap
        };

        var mapper = RecipeMapping.ToDetailedDto(ImageBaseUrl).Compile();
        _actual = mapper(sut);
    }

    [Test]
    public void MapsId() => Assert.That(_actual.Id, Is.EqualTo(ExpectedId));

    [Test]
    public void MapsTitle() => Assert.That(_actual.Title, Is.EqualTo(ExpectedTitle));

    [Test]
    public void MapsSummary() => Assert.That(_actual.Summary, Is.EqualTo(ExpectedSummary));

    [Test]
    public void MapsThumbnailWithBaseUrlPrefix() =>
        Assert.That(_actual.Thumbnail,Is.EqualTo(ImageBaseUrl + ExpectedThumbnail));

    [Test]
    public void MapsVideoPathWithBaseUrlPrefix() =>
        Assert.That(_actual.VideoPath, Is.EqualTo(ImageBaseUrl + ExpectedVideoPath));

    [Test]
    public void MapsPreparationTimeInMinutes() =>
        Assert.That(_actual.PreparationTimeInMinutes, Is.EqualTo(ExpectedPreparationTime));

    [Test]
    public void MapsCookingTimeInMinutes() =>
        Assert.That(_actual.CookingTimeInMinutes, Is.EqualTo(ExpectedCookingTime));

    [Test]
    public void MapsBakingTimeInMinutes() =>
        Assert.That(_actual.BakingTimeInMinutes, Is.EqualTo(ExpectedBakingTime));

    [Test]
    public void MapsServings() => Assert.That(_actual.Servings, Is.EqualTo(ExpectedServings));

    [Test]
    public void MapsIsVegan() => Assert.That(_actual.IsVegan, Is.EqualTo(ExpectedIsVegan));

    [Test]
    public void MapsIsVegetarian() => Assert.That(_actual.IsVegetarian, Is.EqualTo(ExpectedIsVegetarian));

    [Test]
    public void MapsIsCheap() => Assert.That(_actual.IsCheap, Is.EqualTo(ExpectedIsCheap));

    [Test]
    public void MapsIsHealthy() => Assert.That(_actual.IsHealthy, Is.EqualTo(ExpectedIsHealthy));

    [Test]
    public void MapsIsDairyFree() => Assert.That(_actual.IsDairyFree, Is.EqualTo(ExpectedIsDairyFree));

    [Test]
    public void MapsIsGlutenFree() => Assert.That(_actual.IsGlutenFree, Is.EqualTo(ExpectedIsGlutenFree));

    [Test]
    public void MapsIsLowFodmap() => Assert.That(_actual.IsLowFodmap, Is.EqualTo(ExpectedIsLowFodmap));
}

