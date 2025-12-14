using SharedCookbook.Domain.Exceptions;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.ValueObjects;

public class ColourTests
{
    [Test]
    public void ShouldReturnCorrectColourCode()
    {
        var code = "#FFFFFF";

        var colour = Colour.From(code);

        Assert.That(colour.Code, Is.EqualTo(code));
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
        string code = Colour.White;

        code.Should().Be("#FFFFFF");
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedColourCode()
    {
        var colour = (Colour)"#FFFFFF";

        colour.Should().Be(Colour.White);
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        FluentActions.Invoking(() => Colour.From("##FF33CC"))
            .Should().Throw<UnsupportedColourException>();
    }
}
