using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.IntegrationTests;

public class InvitationResponderTests
{
    private const string MockUserId = "mockUserId";
    private Mock<IApplicationDbContext> _context = null!;
    private Mock<IUser> _user = null!;
    private Mock<TimeProvider> _clock = null!;
    private InvitationResponder _sut = null!;
    private CookbookInvitation _defaultInvitation = null!;

    [SetUp]
    public void Setup()
    {
        _context = new Mock<IApplicationDbContext>();
        _user = new Mock<IUser>();
        _clock = new Mock<TimeProvider>();
        _sut = new InvitationResponder(_context.Object, _user.Object, _clock.Object);
        _defaultInvitation = new CookbookInvitation();
    }

    [Test]
    public async Task WhenCurrentStatusIsSameAsUpdatedReturnsId()
    {
        int actual = await _sut.Respond(_defaultInvitation, _defaultInvitation.Status);

        Assert.That(actual, Is.EqualTo(_defaultInvitation.Id));
    }

    [Test]
    public async Task ThrowArgumentNullExceptionWhenUserIdIsNull()
    {
        await FluentActions.Invoking(() => _sut.Respond(_defaultInvitation, InvitationStatus.Revoked))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [TestCase(InvitationStatus.Error)]
    [TestCase(InvitationStatus.Active)]
    [TestCase(InvitationStatus.Revoked)]
    public async Task ThrowsNotSupportedExceptionForUnsupportedStatusUpdate(InvitationStatus status)
    {
        _user.SetupGet(user => user.Id).Returns(MockUserId);
        
        await FluentActions.Invoking(() => _sut.Respond(_defaultInvitation, status))
            .Should().ThrowAsync<NotSupportedException>();
    }

    // we can't easily include this case in the test above due to "Unknown" being the default status for an Invitation
    [Test]
    public async Task ThrowsNotSupportedExceptionWhenUpdatingToUnknownFromDifferentStatus()
    {
        _user.SetupGet(user => user.Id).Returns(MockUserId);
        _defaultInvitation.Reject(It.IsAny<DateTimeOffset>());
        
        await FluentActions.Invoking(() => _sut.Respond(_defaultInvitation, InvitationStatus.Active))
            .Should().ThrowAsync<NotSupportedException>();
    }
}
