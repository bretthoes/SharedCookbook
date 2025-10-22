using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Infrastructure.IntegrationTests;

public class InvitationResponderTests
{
    private Mock<IApplicationDbContext> _context = null!;
    private Mock<IUser> _user = null!;
    private Mock<TimeProvider> _clock = null!;

    [SetUp]
    public void Setup()
    {
        _context = new Mock<IApplicationDbContext>();
        _user = new Mock<IUser>();
        _clock = new Mock<TimeProvider>();
    }

    [Test]
    public async Task WhenCurrentStatusIsSameAsUpdatedReturnsId()
    {
        var sut = new InvitationResponder(_context.Object, _user.Object, _clock.Object);
        var invitation = new CookbookInvitation();

        var actual = await sut.Respond(invitation, invitation.Status);

        Assert.That(actual, Is.EqualTo(invitation.Id));
    }

    [Test]
    public async Task ThrowArgumentNullExceptionWhenUserIdIsNull()
    {
        var sut = new InvitationResponder(_context.Object, _user.Object, _clock.Object);
        var invitation = new CookbookInvitation();

        await FluentActions.Invoking(() => sut.Respond(invitation, InvitationStatus.Revoked))
            .Should().ThrowAsync<ArgumentNullException>();
    }
}
