using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.UnitTests.Common.Mappings.RecipeDirectionMappingTests;

public class ToEntitiesTests
{
    private const int ExpectedId = 1;
    private const int ExpectedOrdinal = 2;
    private const string ExpectedText = "TestDirectionText";
    private const string ExpectedImage = "TestDirectionImage";

    private RecipeDirection _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var sut = new List<RecipeDirectionDto>
        {
            new() { Id = ExpectedId, Text = ExpectedText, Ordinal = ExpectedOrdinal, Image = ExpectedImage }
        };

        _actual = sut.ToEntities().Single();
    }

    [Test]
    public void MapsId() => Assert.That(_actual.Id, Is.EqualTo(ExpectedId));

    [Test]
    public void MapsText() => Assert.That(_actual.Text, Is.EqualTo(ExpectedText));

    [Test]
    public void MapsOrdinal() => Assert.That(_actual.Ordinal, Is.EqualTo(ExpectedOrdinal));

    [Test]
    public void MapsImage() => Assert.That(_actual.Image, Is.EqualTo(ExpectedImage));

    [Test]
    public void EmptyCollectionReturnsEmpty()
    {
        var actual = new List<RecipeDirectionDto>().ToEntities();

        Assert.That(actual, Is.Empty);
    }
}

