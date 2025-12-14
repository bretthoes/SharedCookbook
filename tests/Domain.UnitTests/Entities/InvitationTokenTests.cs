using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class InvitationTokenTests
{
    [Test]
    public void TokenCreatedOverTwoWeeksAgoIsRedeemable()
    {
        var sut = InvitationToken.IssueNewToken(digest: It.IsAny<TokenDigest>(), cookbookId: It.IsAny<int>());
        
        sut.Created = DateTimeOffset.Now.AddDays(-15);
        
        Assert.That(sut.IsRedeemable, Is.False);
    }

    [Test]
    public void TokenCreatedWithinTwoWeeksIsNotExpired()
    {
        var sut = InvitationToken.IssueNewToken(digest: It.IsAny<TokenDigest>(), cookbookId: It.IsAny<int>());
        
        sut.Created = DateTimeOffset.Now.AddDays(-13);

        Assert.That(sut.IsRedeemable, Is.True);
    }
}
