using SharedCookbook.Application.Common.Behaviours;
using SharedCookbook.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
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
        var id = 1;
        _user.Setup(x => x.Id).Returns(id);

        var requestLogger = new LoggingBehaviour<CreateCookbookCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateCookbookCommand { Title = "title" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<int>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<CreateCookbookCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new CreateCookbookCommand { Title = "title" }, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<int>()), Times.Never);
    }
}
