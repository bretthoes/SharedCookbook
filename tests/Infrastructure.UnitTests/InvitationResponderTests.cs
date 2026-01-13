using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.UnitTests;

public class InvitationResponderTests
{
    private const string MockUserId = "mockUserId";
    private Mock<IApplicationDbContext> _context = null!;
    private Mock<IUser> _user = null!;
    private Mock<TimeProvider> _clock = null!;
    private InvitationResponder _sut = null!;
    private CookbookInvitation _activeInvitation = null!;

    [SetUp]
    public void Setup()
    {
        _context = new Mock<IApplicationDbContext>();
        _user = new Mock<IUser>();
        _clock = new Mock<TimeProvider>();
        _sut = new InvitationResponder(_context.Object, _user.Object, _clock.Object);
        _activeInvitation = CookbookInvitation.Create(It.IsAny<int>(), MockUserId);
        _user.SetupGet(user => user.Id).Returns(MockUserId);
    }

    [Test]
    public async Task WhenCurrentStatusIsSameAsUpdatedReturnsId()
    {
        int actual = await _sut.Respond(_activeInvitation, _activeInvitation.Status);

        Assert.That(actual, Is.EqualTo(_activeInvitation.Id));
    }

    [Test]
    public void ThrowArgumentNullExceptionWhenUserIdIsNull()
    {
        _user.SetupGet(user => user.Id).Returns((string)null!);

        Assert.ThrowsAsync<ArgumentNullException>(() =>
            _sut.Respond(_activeInvitation, InvitationStatus.Revoked));
    }

    [TestCase(InvitationStatus.Error)]
    [TestCase(InvitationStatus.Unknown)]
    [TestCase(InvitationStatus.Revoked)]
    public void ThrowsNotSupportedExceptionForUnsupportedStatusUpdate(InvitationStatus status)
    {
        Assert.ThrowsAsync<NotSupportedException>(() => _sut.Respond(_activeInvitation, status));
    }

    [Test]
    public async Task InvitationStatusIsAcceptedAfterBeingAccepted()
    {
        await _sut.Respond(_activeInvitation, InvitationStatus.Accepted);

        Assert.That(_activeInvitation.Status, Is.EqualTo(InvitationStatus.Accepted));
    }

    [Test]
    public async Task InvitationStatusIsRejectedAfterBeingRejected()
    {
        await _sut.Respond(_activeInvitation, InvitationStatus.Rejected);

        Assert.That(_activeInvitation.Status, Is.EqualTo(InvitationStatus.Rejected));
    }

    [Test]
    public async Task InactiveInvitationStatusIsUnchangedAfterBeingAccepted()
    {
        var defaultInvitation = new CookbookInvitation();
        var expected = defaultInvitation.Status;

        await _sut.Respond(defaultInvitation, InvitationStatus.Accepted);

        Assert.That(defaultInvitation.Status, Is.EqualTo(expected));
    }

    [Test]
    public async Task InactiveInvitationStatusIsUnchangedAfterBeingRejected()
    {
        var defaultInvitation = new CookbookInvitation();
        var expected = defaultInvitation.Status;

        await _sut.Respond(defaultInvitation, InvitationStatus.Rejected);

        Assert.That(defaultInvitation.Status, Is.EqualTo(expected));
    }
}
