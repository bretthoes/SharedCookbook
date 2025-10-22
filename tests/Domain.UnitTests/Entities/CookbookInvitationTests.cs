using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Domain.UnitTests.Entities;

public class CookbookInvitationTests
{
    [Test]
    public void CreatedInvitationShouldHaveActiveStatus()
    {
        const InvitationStatus expected = InvitationStatus.Active;

        var invitation = CookbookInvitation.Create(It.IsAny<int>(), It.IsAny<string>());
        
        var actual = invitation.Status;
        
        actual.Should().Be(expected);
    }
    
}
