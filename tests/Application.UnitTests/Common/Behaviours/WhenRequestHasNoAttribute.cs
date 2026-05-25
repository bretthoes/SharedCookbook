using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

public class WhenRequestHasNoAttribute
{
    private Mock<ILogger<CreateCookbookCommand>> _logger = null!;
    private PerformanceBehaviour<CreateCookbookCommand, int> _sut = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateCookbookCommand>>();
        _sut = new PerformanceBehaviour<CreateCookbookCommand, int>(
            _logger.Object,
            Mock.Of<IUser>(),
            Mock.Of<IIdentityService>(),
            PerformanceBehaviourTestHelpers.CreateTimeProvider(600));
    }

    [Test]
    public async Task ShouldLogWarningWhenElapsedExceedsDefaultThreshold()
    {
        await _sut.Handle(
            new CreateCookbookCommand(Title: "title"),
            PerformanceBehaviourTestHelpers.ImmediateHandler(1),
            CancellationToken.None);

        PerformanceBehaviourTestHelpers.VerifyLogWarning(_logger, Times.Once());
    }
}
