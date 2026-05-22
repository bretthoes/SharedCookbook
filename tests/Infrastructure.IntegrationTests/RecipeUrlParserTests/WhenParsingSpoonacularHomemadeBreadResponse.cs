using System.Text.Json;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Infrastructure.RecipeUrlParser.Models;

namespace SharedCookbook.Infrastructure.IntegrationTests.RecipeUrlParserTests;

public class WhenParsingSpoonacularHomemadeBreadResponse
{
    private const string FixturePath =
        "RecipeUrlParserTests/Fixtures/spoonacular-extract-homemade-bread-analyzed-instructions.json";

    private const int ExpectedStepCount = 8;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private List<RecipeDirectionDto> _actualDirections = [];
    private List<string> _expectedStepTexts = [];

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        string json = File.ReadAllText(FixturePath);
        _expectedStepTexts = ExtractExpectedStepTexts(json);

        var apiResponse = JsonSerializer.Deserialize<RecipeApiResponse>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize Spoonacular fixture.");

        _actualDirections = apiResponse.ToDto(imageKey: null).Directions.ToList();
    }

    private static List<string> ExtractExpectedStepTexts(string json)
    {
        using var document = JsonDocument.Parse(json);

        return document.RootElement
            .GetProperty("analyzedInstructions")
            .EnumerateArray()
            .SelectMany(analyzedInstruction => analyzedInstruction.GetProperty("steps").EnumerateArray())
            .Select(step => step.GetProperty("step").GetString()!)
            .ToList();
    }

    [Test]
    public void ShouldHaveExpectedCount() => Assert.That(_actualDirections, Has.Count.EqualTo(ExpectedStepCount));

    [Test]
    public void Step0_HasExpectedText() => Assert.That(_actualDirections[0].Text, Is.EqualTo(_expectedStepTexts[0]));

    [Test]
    public void Step0_HasExpectedOrdinal() => Assert.That(_actualDirections[0].Ordinal, Is.EqualTo(1));

    [Test]
    public void Step1_HasExpectedText() => Assert.That(_actualDirections[1].Text, Is.EqualTo(_expectedStepTexts[1]));

    [Test]
    public void Step1_HasExpectedOrdinal() => Assert.That(_actualDirections[1].Ordinal, Is.EqualTo(2));

    [Test]
    public void Step2_HasExpectedText() => Assert.That(_actualDirections[2].Text, Is.EqualTo(_expectedStepTexts[2]));

    [Test]
    public void Step2_HasExpectedOrdinal() => Assert.That(_actualDirections[2].Ordinal, Is.EqualTo(3));

    [Test]
    public void Step3_HasExpectedText() => Assert.That(_actualDirections[3].Text, Is.EqualTo(_expectedStepTexts[3]));

    [Test]
    public void Step3_HasExpectedOrdinal() => Assert.That(_actualDirections[3].Ordinal, Is.EqualTo(4));

    [Test]
    public void Step4_HasExpectedText() => Assert.That(_actualDirections[4].Text, Is.EqualTo(_expectedStepTexts[4]));

    [Test]
    public void Step4_HasExpectedOrdinal() => Assert.That(_actualDirections[4].Ordinal, Is.EqualTo(5));

    [Test]
    public void Step5_HasExpectedText() => Assert.That(_actualDirections[5].Text, Is.EqualTo(_expectedStepTexts[5]));

    [Test]
    public void Step5_HasExpectedOrdinal() => Assert.That(_actualDirections[5].Ordinal, Is.EqualTo(6));

    [Test]
    public void Step6_HasExpectedText() => Assert.That(_actualDirections[6].Text, Is.EqualTo(_expectedStepTexts[6]));

    [Test]
    public void Step6_HasExpectedOrdinal() => Assert.That(_actualDirections[6].Ordinal, Is.EqualTo(7));

    [Test]
    public void Step7_HasExpectedText() => Assert.That(_actualDirections[7].Text, Is.EqualTo(_expectedStepTexts[7]));

    [Test]
    public void Step7_HasExpectedOrdinal() => Assert.That(_actualDirections[7].Ordinal, Is.EqualTo(8));
}
