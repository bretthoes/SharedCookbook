using SharedCookbook.Application.Contracts;
using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.UnitTests.RecipeUrlParserTests.ExtractDirectionsTests;

public class WhenStepsAreNumbered
{
    private const int ExpectedStepCount = 5;
    private const string ExpectedStep0Text = "1. Place oven on broil";
    private const string ExpectedStep1Text 
        = "2. In a bowl mix together dry rub: chili powder, paprika, coconut sugar, and salt";
    private const string ExpectedStep2Text = "3. Cover salmon filets with dry rub";
    private const string ExpectedStep3Text = "4. Place salmon on lined baking sheet and broil for 7-8 minutes";
    private const string ExpectedStep4Text = "5. Brush maple syrup over salmon and broil for an additional 1-2 minutes";
    private const string InstructionsWithNumberedStepsNoNewlines =
        ExpectedStep0Text + ExpectedStep1Text + ExpectedStep2Text + ExpectedStep3Text + ExpectedStep4Text;

    private const int ExpectedStep0Ordinal = 1;
    private const int ExpectedStep1Ordinal = 2;
    private const int ExpectedStep2Ordinal = 3;
    private const int ExpectedStep3Ordinal = 4;
    private const int ExpectedStep4Ordinal = 5;

    private List<RecipeDirectionDto> _actual = [];

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _actual = RecipeApiResponseExtensions.ExtractDirections(InstructionsWithNumberedStepsNoNewlines);
    }

    [Test]
    public void ShouldHaveExpectedCount() => Assert.That(_actual, Has.Count.EqualTo(ExpectedStepCount));

    [Test]
    public void Step0_HasExpectedText() => Assert.That(_actual[0].Text, Is.EqualTo(ExpectedStep0Text));

    [Test]
    public void Step0_HasExpectedOrdinal()
        => Assert.That(_actual[0].Ordinal, Is.EqualTo(ExpectedStep0Ordinal));

    [Test]
    public void Step1_HasExpectedText() => Assert.That(_actual[1].Text, Is.EqualTo(ExpectedStep1Text));

    [Test]
    public void Step1_HasExpectedOrdinal()
        => Assert.That(_actual[1].Ordinal, Is.EqualTo(ExpectedStep1Ordinal));

    [Test]
    public void Step2_HasExpectedText() => Assert.That(_actual[2].Text, Is.EqualTo(ExpectedStep2Text));

    [Test]
    public void Step2_HasExpectedOrdinal()
        => Assert.That(_actual[2].Ordinal, Is.EqualTo(ExpectedStep2Ordinal));

    [Test]
    public void Step3_HasExpectedText() => Assert.That(_actual[3].Text, Is.EqualTo(ExpectedStep3Text));

    [Test]
    public void Step3_HasExpectedOrdinal()
        => Assert.That(_actual[3].Ordinal, Is.EqualTo(ExpectedStep3Ordinal));

    [Test]
    public void Step4_HasExpectedText() => Assert.That(_actual[4].Text, Is.EqualTo(ExpectedStep4Text));

    [Test]
    public void Step4_HasExpectedOrdinal()
        => Assert.That(_actual[4].Ordinal, Is.EqualTo(ExpectedStep4Ordinal));
}
