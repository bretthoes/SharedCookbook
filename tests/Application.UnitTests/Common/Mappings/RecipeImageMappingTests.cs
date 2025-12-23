using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.UnitTests.Common.Mappings;

public class RecipeImageMappingTests
{
    private const int ExpectedId = 1;
    private const int ExpectedOrdinal = 2;
    private const string ExpectedName = "TestImageName";
    private const string ImageBaseUrl = "https://example.com/images/";

    private RecipeImageDto _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var sut = new List<RecipeImage>
        {
            new() { Id = ExpectedId, Name = ExpectedName, Ordinal = ExpectedOrdinal }
        };

        _actual = sut.ToDtos(ImageBaseUrl).Single();
    }

    [Test]
    public void MapsId() => Assert.That(_actual.Id, Is.EqualTo(ExpectedId));

    [Test]
    public void MapsNameWithBaseUrlPrefix() => Assert.That(_actual.Name, Is.EqualTo(ImageBaseUrl + ExpectedName));

    [Test]
    public void MapsOrdinal() => Assert.That(_actual.Ordinal, Is.EqualTo(ExpectedOrdinal));

    [Test]
    public void EmptyCollectionReturnsEmpty()
    {
        var actual = new List<RecipeImage>().ToDtos(ImageBaseUrl);

        Assert.That(actual, Is.Empty);
    }
}

