using SharedCookbook.Application.Common.Mappings;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.UnitTests.Common.Mappings;

public class RecipeIngredientMappingTests
{
    private const int ExpectedId = 1;
    private const int ExpectedOrdinal = 2;
    private const string ExpectedName = "TestIngredientName";
    private const bool ExpectedOptional = true;

    private RecipeIngredientDto _actual = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var sut = new List<RecipeIngredient>
        {
            new() { Id = ExpectedId, Name = ExpectedName, Ordinal = ExpectedOrdinal, Optional = ExpectedOptional }
        };

        _actual = sut.ToDtos().Single();
    }

    [Test]
    public void MapsId() => Assert.That(_actual.Id, Is.EqualTo(ExpectedId));

    [Test]
    public void MapsName() => Assert.That(_actual.Name, Is.EqualTo(ExpectedName));

    [Test]
    public void MapsOrdinal() => Assert.That(_actual.Ordinal, Is.EqualTo(ExpectedOrdinal));

    [Test]
    public void MapsOptional() => Assert.That(_actual.Optional, Is.EqualTo(ExpectedOptional));
    
    [Test]
    public void EmptyCollectionReturnsEmpty()
    {
        var actual = new List<RecipeIngredient>().ToDtos();

        Assert.That(actual, Is.Empty);
    }
}
