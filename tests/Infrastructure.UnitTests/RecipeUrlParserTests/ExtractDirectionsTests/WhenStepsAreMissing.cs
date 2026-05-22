using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.UnitTests.RecipeUrlParserTests.ExtractDirectionsTests;

public class WhenStepsAreMissing
{
    [TestCase("")]
    [TestCase("   \n   ")]
    [TestCase("   \n\n   ")]
    [TestCase(null)]
    public void ShouldBeEmpty(string? input)
    {
        var actual = RawInstructionDirectionsParser.Parse(input);

        Assert.That(actual, Is.Empty);
    }
}
