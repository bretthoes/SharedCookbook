using SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;
using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.FunctionalTests.Memberships.Commands;

using static Testing;

public class UpdateMembershipTests : BaseTestFixture
{
    [Test]
    public async Task ShouldUpdateMembership()
    {
        // This test should simulate a cookbook with two members, where one is the owner and the other a contributor.
        // When the contributor is promoted to owner BY the owner, the original owner should be demoted to contributor
        string newOwnerUserId = await RunAsUserAsync("test@test.com", "Testing1234!", []);
        var contributorMembership = new CookbookMembership { CreatedBy = newOwnerUserId };

        string originalOwnerUserId = await RunAsDefaultUserAsync();
        var ownerMembership = CookbookMembership.NewOwner();
        ownerMembership.CreatedBy = originalOwnerUserId;

        await AddAsync(new Cookbook
        {
            Title = "Existing Cookbook",
            CreatedBy = originalOwnerUserId,
            Memberships = [ownerMembership, contributorMembership]
        });

        // Act: promote second member to owner
        await SendAsync(new UpdateMembershipCommand { Id = 2, IsCreator = true });

        var demotedOwner = await FindAsync<CookbookMembership>(1);
        var promotedOwner = await FindAsync<CookbookMembership>(2);

        demotedOwner!.IsOwner.Should().BeFalse();
        promotedOwner!.IsOwner.Should().BeTrue();

        demotedOwner.LastModifiedBy.Should().Be(originalOwnerUserId);
        promotedOwner.LastModifiedBy.Should().Be(originalOwnerUserId);
        demotedOwner.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    }
}
