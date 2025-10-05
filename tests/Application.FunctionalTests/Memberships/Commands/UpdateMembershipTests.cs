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
    public async Task ShouldUpdateCookbook()
    {
        string userId = await RunAsDefaultUserAsync();

        // Create an owner membership and regular membership
        // Update regular to owner
        // Verify the previous owner gets demoted

        int cookbookId = await SendAsync(new CreateCookbookCommand("Title"));
        var creator = await FindAsync<CookbookMembership>(1);

        string otherUserId = await RunAsUserAsync("test@test.com", "Testing1234!", []);
        var member = new CookbookMembership
        {
            CookbookId = cookbookId,
            Id = 0,
            CreatedBy = otherUserId,
        };
        member.SetPermissions(Permissions.Contributor);
        await AddAsync(member);

        var command = new UpdateMembershipCommand
        {
            Id = member.Id,
            IsCreator = true
        };

        await SendAsync(command);

        var cookbook = await FindAsync<Cookbook>(cookbookId);
        // shouldn't be creator anymore
        creator = await FindAsync<CookbookMembership>(1);

        cookbook.Should().NotBeNull();
        //cookbook!.Title.Should().Be(command.Title);
        cookbook.LastModifiedBy.Should().NotBeNull();
        cookbook.LastModifiedBy.Should().Be(userId);
        cookbook.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
