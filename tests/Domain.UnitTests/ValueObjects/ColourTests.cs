using SharedCookbook.Domain.Exceptions;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Test]
    public void ShouldReturnCorrectColourCode()
    {
        const string expected = "#FFFFFF";

        var colour = Colour.From(expected);

        Assert.That(colour.Code, Is.EqualTo(expected));
    }

    [Test]
    public void ToStringReturnsCode()
    {
        var colour = Colour.White;

        Assert.That(colour.Code.ToString(), Is.EqualTo(colour.Code));
    }

    [Test]
    public void ShouldPerformImplicitConversionToColourCodeString()
    {
        const string expected = "#FFFFFF";
        
        string code = Colour.White;

        Assert.That(code, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        string expected = Colour.White;
        
        var actual = (Colour)"#FFFFFF";

        Assert.That(actual.Code, Is.EqualTo(expected));
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        Assert.Throws<UnsupportedColourException>(() => Colour.From("#FF33CC"));
    }
}
