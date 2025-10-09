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
        // Arrange: two members in one cookbook
        string newOwnerUserId = await RunAsUserAsync("test@test.com", "Testing1234!", []);
        var contributorMembership = new CookbookMembership { CreatedBy = newOwnerUserId };

        string originalOwnerUserId = await RunAsDefaultUserAsync(); // becomes current actor
        var ownerMembership = CookbookMembership.NewOwner();
        ownerMembership.CreatedBy = originalOwnerUserId;

        await AddAsync(new Cookbook
        {
            Title = "Existing Cookbook",
            CreatedBy = originalOwnerUserId,
            Memberships = [ownerMembership, contributorMembership]
        });

        // Act: promote contributor to owner (performed by original owner)
        await SendAsync(new UpdateMembershipCommand { Id = contributorMembership.Id, IsCreator = true });

        // Assert: resolve by CreatedBy (stable) instead of PKs
        var memberships = await ListAsync<CookbookMembership>();
        var demotedOwner = memberships.Single(m => m.CreatedBy == originalOwnerUserId);
        var promotedOwner = memberships.Single(m => m.CreatedBy == newOwnerUserId);

        demotedOwner.IsOwner.Should().BeFalse();
        demotedOwner.LastModifiedBy.Should().Be(originalOwnerUserId);
        demotedOwner.LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, precision: TimeSpan.FromSeconds(10));

        promotedOwner.IsOwner.Should().BeTrue();
        promotedOwner.LastModifiedBy.Should().Be(originalOwnerUserId);
        promotedOwner.LastModified.Should().BeCloseTo(DateTimeOffset.UtcNow, precision: TimeSpan.FromSeconds(10));
    }
}
