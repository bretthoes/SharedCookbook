using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.UnitTests.Common.Mappings.RecipeDirectionMappingTests;

public class ToDtosTests
{
    private const int ExpectedOrdinal = 2;
    private const string ExpectedText = "TestDirectionText";
    private const string ExpectedImage = "TestDirectionImage";
    private const string ImageBaseUrl = "https://example.com/images/";

    private RecipeDirectionDto _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var sut = new List<RecipeDirection>
        {
            new() { Text = ExpectedText, Ordinal = ExpectedOrdinal, Image = ExpectedImage }
        };

        _actual = sut.ToDtos(ImageBaseUrl).Single();
    }

    [Test]
    public void MapsText() => Assert.That(_actual.Text, Is.EqualTo(ExpectedText));

    [Test]
    public void MapsOrdinal() => Assert.That(_actual.Ordinal, Is.EqualTo(ExpectedOrdinal));

    [Test]
    public void MapsImageWithBaseUrlPrefix() => Assert.That(_actual.Image, Is.EqualTo(ImageBaseUrl + ExpectedImage));

    [Test]
    public void EmptyCollectionReturnsEmpty()
    {
        var actual = new List<RecipeDirection>().ToDtos(ImageBaseUrl);

        Assert.That(actual, Is.Empty);
    }
}

