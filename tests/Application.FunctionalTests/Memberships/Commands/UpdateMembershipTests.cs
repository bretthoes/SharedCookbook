using SharedCookbook.Application.Memberships.Commands.UpdateMembership;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Application.FunctionalTests.Memberships.Commands;

using static Testing;

public class UpdateMembershipTests : BaseTestFixture
{
    [Test, Ignore("causing concurrency issues running as two different users")]
    public async Task ShouldUpdateMembership()
    {
        // Arrange: two members in one cookbook
        string newOwnerUserId = await RunAsUserAsync("test@test.com", "Testing1234!", []);
        var contributorMembership = new CookbookMembership { CreatedBy = newOwnerUserId };

        string originalOwnerUserId = await RunAsDefaultUserAsync(); // becomes current actor
        var ownerMembership = CookbookMembership.NewOwner(originalOwnerUserId);

        await AddAsync(new Cookbook
        {
            Title = "Existing Cookbook",
            CreatedBy = originalOwnerUserId,
            Memberships = [ownerMembership, contributorMembership]
        });

        // Act: promote contributor to owner (performed by original owner)
        await SendAsync(new UpdateMembershipCommand
        {
            Id = contributorMembership.Id,
            IsOwner = true,
            CanAddRecipe = false,
            CanUpdateRecipe = false,
            CanDeleteRecipe = false,
            CanSendInvite = false,
            CanRemoveMember = false,
            CanEditCookbookDetails = false
        });

        // Assert: resolve by CreatedBy (stable) instead of PKs
        var memberships = await ListAsync<CookbookMembership>();
        var demotedOwner = memberships.Single(m => m.CreatedBy == originalOwnerUserId);
        var promotedOwner = memberships.Single(m => m.CreatedBy == newOwnerUserId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(demotedOwner.IsOwner, Is.False);
            Assert.That(demotedOwner.LastModifiedBy, Is.EqualTo(originalOwnerUserId));
            Assert.That(demotedOwner.LastModified, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(10)));

            Assert.That(promotedOwner.IsOwner, Is.True);
            Assert.That(promotedOwner.LastModifiedBy, Is.EqualTo(originalOwnerUserId));
            Assert.That(promotedOwner.LastModified, Is.EqualTo(DateTimeOffset.UtcNow).Within(TimeSpan.FromSeconds(10)));
        }
    }
}
