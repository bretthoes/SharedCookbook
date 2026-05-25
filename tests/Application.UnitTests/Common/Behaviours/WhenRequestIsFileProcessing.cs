using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

public class WhenRequestIsFileProcessing
{
    private Mock<ILogger<CreateImagesCommand>> _logger = null!;
    private PerformanceBehaviour<CreateImagesCommand, string[]> _sut = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateImagesCommand>>();
    }

    [Test]
    public async Task ShouldNotLogWarningWhenElapsedIsWithinThreshold()
    {
        _sut = new PerformanceBehaviour<CreateImagesCommand, string[]>(
            _logger.Object,
            Mock.Of<IUser>(),
            Mock.Of<IIdentityService>(),
            PerformanceBehaviourTestHelpers.CreateTimeProvider(2_000));

        await _sut.Handle(
            new CreateImagesCommand(Files: null!),
            PerformanceBehaviourTestHelpers.ImmediateHandler(Array.Empty<string>()));

        PerformanceBehaviourTestHelpers.VerifyLogWarning(_logger, Times.Never());
    }

    [Test]
    public async Task ShouldLogWarningWhenElapsedExceedsThreshold()
    {
        _sut = new PerformanceBehaviour<CreateImagesCommand, string[]>(
            _logger.Object,
            Mock.Of<IUser>(),
            Mock.Of<IIdentityService>(),
            PerformanceBehaviourTestHelpers.CreateTimeProvider(4_000));

        await _sut.Handle(
            new CreateImagesCommand(Files: null!),
            PerformanceBehaviourTestHelpers.ImmediateHandler(Array.Empty<string>()));

        PerformanceBehaviourTestHelpers.VerifyLogWarning(_logger, Times.Once());
    }
}
