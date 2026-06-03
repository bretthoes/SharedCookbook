using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class InvitationTokenTests 
{
    [Test]
    public void TokenCreatedOverTwoWeeksAgoIsNotRedeemable()
    {
        var sut = InvitationToken.IssueNewToken(digest: It.IsAny<TokenDigest>(), cookbookId: It.IsAny<Guid>());
        
        sut.Created = DateTimeOffset.Now.AddDays(-15);
        
        Assert.That(sut.IsRedeemable, Is.False);
    }

    [Test]
    public void TokenCreatedWithinTwoWeeksIsRedeemable()
    {
        var sut = InvitationToken.IssueNewToken(digest: It.IsAny<TokenDigest>(), cookbookId: It.IsAny<Guid>());
        
        sut.Created = DateTimeOffset.Now.AddDays(-13);

        Assert.That(sut.IsRedeemable, Is.True);
    }
}

