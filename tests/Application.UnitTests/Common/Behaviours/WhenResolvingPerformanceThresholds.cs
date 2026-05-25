using SharedCookbook.Application.Common.Performance;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Images.Commands.CreateImages;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

public class WhenResolvingPerformanceThresholds
{
    [Test]
    public void ShouldReturnDefaultForUnmarkedRequest() =>
        Assert.That(
            PerformanceThresholds.For(typeof(CreateCookbookCommand)),
            Is.EqualTo(PerformanceThresholds.DefaultMilliseconds));

    [Test]
    public void ShouldReturnExternalApiThresholdForMarkedRequest() =>
        Assert.That(
            PerformanceThresholds.For(typeof(ParseRecipeFromVoiceCommand)),
            Is.EqualTo(PerformanceThresholds.ExternalApiMilliseconds));

    [Test]
    public void ShouldReturnFileProcessingThresholdForMarkedRequest() =>
        Assert.That(
            PerformanceThresholds.For(typeof(CreateImagesCommand)),
            Is.EqualTo(PerformanceThresholds.FileProcessingMilliseconds));
}
