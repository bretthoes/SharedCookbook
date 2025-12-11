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

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void InstantiatedInvitationShouldHaveUnknownStatus()
    {
        const InvitationStatus expected = InvitationStatus.Unknown;

        var actual = (new CookbookInvitation()).Status;

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void InvitationStatusIsAcceptedAfterAccept()
    {
        const InvitationStatus expected = InvitationStatus.Accepted;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Accept(timestamp: It.IsAny<DateTimeOffset>(), acceptedBy: It.IsAny<string>());

        Assert.That(sut.Status, Is.EqualTo(expected));
    }
    
    [Test]
    public void DomainEventsNotEmptyAfterAccept()
    {
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Accept(timestamp: It.IsAny<DateTimeOffset>(), acceptedBy: It.IsAny<string>());

        Assert.That(sut.DomainEvents, Is.Not.Empty);
    }
    
    [Test]
    public void ResponseDateIsUpdatedAfterAccept()
    {
        var expected = DateTimeOffset.Now;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Accept(timestamp: expected, acceptedBy: It.IsAny<string>());

        Assert.That(sut.ResponseDate, Is.EqualTo(expected).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void InvitationStatusIsRejectedAfterReject()
    {
        const InvitationStatus expected = InvitationStatus.Rejected;
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Reject(timestamp: It.IsAny<DateTimeOffset>());

        Assert.That(sut.Status, Is.EqualTo(expected));
    }

    [Test]
    public void DomainEventsNotEmptyAfterReject()
    {
        var sut = CookbookInvitation.Create(cookbookId: It.IsAny<int>(), recipientId: It.IsAny<string>());

        sut.Reject(timestamp: It.IsAny<DateTimeOffset>());

        Assert.That(sut.DomainEvents, Is.Not.Empty);
    }
    
    [Test]
    public void CreatedInvitationIsForSpecifiedUser()
    {
        string expected = Guid.NewGuid().ToString();
        
        var actual = CookbookInvitation
            .Create(cookbookId: It.IsAny<int>(), recipientId: expected)
            .IsNotFor(expected);
        
        Assert.That(actual, Is.False);
    }
}
