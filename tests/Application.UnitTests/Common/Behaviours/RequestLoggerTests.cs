using Microsoft.Extensions.Logging;
using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

namespace SharedCookbook.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateCookbookCommand>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateCookbookCommand>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(user => user.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<CreateCookbookCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateCookbookCommand(Title: "title"), CancellationToken.None);

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateCookbookCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateCookbookCommand(Title: "title"), CancellationToken.None);

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
