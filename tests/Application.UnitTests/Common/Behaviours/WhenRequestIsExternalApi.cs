using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Contracts;
using SharedCookbook.Application.Recipes.Commands.ParseRecipeFromVoice;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

public class WhenRequestIsExternalApi
{
    private Mock<ILogger<ParseRecipeFromVoiceCommand>> _logger = null!;
    private PerformanceBehaviour<ParseRecipeFromVoiceCommand, CreateRecipeDto> _sut = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<ParseRecipeFromVoiceCommand>>();
    }

    [Test]
    public async Task ShouldNotLogWarningWhenElapsedIsWithinThreshold()
    {
        _sut = new PerformanceBehaviour<ParseRecipeFromVoiceCommand, CreateRecipeDto>(
            _logger.Object,
            Mock.Of<IUser>(),
            Mock.Of<IIdentityService>(),
            PerformanceBehaviourTestHelpers.CreateTimeProvider(4_000));

        await _sut.Handle(
            new ParseRecipeFromVoiceCommand(Transcript: "test"),
            PerformanceBehaviourTestHelpers.ImmediateHandler(new CreateRecipeDto { Title = "test", CookbookId = Guid.Empty }));

        PerformanceBehaviourTestHelpers.VerifyLogWarning(_logger, Times.Never());
    }

    [Test]
    public async Task ShouldLogWarningWhenElapsedExceedsThreshold()
    {
        _sut = new PerformanceBehaviour<ParseRecipeFromVoiceCommand, CreateRecipeDto>(
            _logger.Object,
            Mock.Of<IUser>(),
            Mock.Of<IIdentityService>(),
            PerformanceBehaviourTestHelpers.CreateTimeProvider(6_000));

        await _sut.Handle(
            new ParseRecipeFromVoiceCommand(Transcript: "test"),
            PerformanceBehaviourTestHelpers.ImmediateHandler(new CreateRecipeDto { Title = "test", CookbookId = Guid.Empty }));

        PerformanceBehaviourTestHelpers.VerifyLogWarning(_logger, Times.Once());
    }
}

