using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookInvitationTests
{
    [Test]
    public void CreatedInvitationShouldHaveActiveStatus()
    {
        const InvitationStatus expected = InvitationStatus.Active;

        var actual = CookbookInvitation
            .Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>())
            .Status;

        actual.Should().Be(expected);
    }

    [Test]
    public void InstantiatedInvitationShouldHaveUnknownStatus()
    {
        const InvitationStatus expected = InvitationStatus.Unknown;

        var actual = (new CookbookInvitation()).Status;

        actual.Should().Be(expected);
    }

    [Test]
    public void InvitationStatusIsAcceptedAfterAccept()
    {
        const InvitationStatus expected = InvitationStatus.Accepted;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Accept(timestamp: It.IsAny<DateTimeOffset>(), acceptedBy: It.IsAny<string>());

        sut.Status.Should().Be(expected);
        sut.DomainEvents.Should().NotBeEmpty();
    }
    
    [Test]
    public void ResponseDateIsUpdatedAfterAccept()
    {
        var expected = DateTimeOffset.Now;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Accept(timestamp: expected, acceptedBy: It.IsAny<string>());

        sut.ResponseDate.Should().BeCloseTo(expected, precision: TimeSpan.FromSeconds(1));
    }

    [Test]
    public void InvitationStatusIsRejectedAfterReject()
    {
        const InvitationStatus expected = InvitationStatus.Rejected;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Reject(timestamp: It.IsAny<DateTimeOffset>());

        sut.Status.Should().Be(expected);
        sut.DomainEvents.Should().NotBeEmpty();
    }

    [Test]
    public void CreatedInvitationIsForSpecifiedUser()
    {
        string expected = Guid.NewGuid().ToString();
        
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: expected);
        
        sut.IsNotFor(expected).Should().BeFalse();
    }
}
