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
        var actingUserId = await RunAsDefaultUserAsync();

        // Arrange: cookbook + initial owner
        var cookbookId = await SendAsync(new CreateCookbookCommand("Title"));
        var originalOwner = await FindAsync<CookbookMembership>(1); // ok in test DB
        originalOwner!.IsOwner.Should().BeTrue();

        // Arrange: second member
        var otherUserId = await RunAsUserAsync("test@test.com", "Testing1234!", []);
        var member = new CookbookMembership { CookbookId = cookbookId, CreatedBy = otherUserId };
        member.SetPermissions(Permissions.Contributor);
        await AddAsync(member);

        // Act: promote second member to owner
        var cmd = new UpdateMembershipCommand
        {
            Id = member.Id, IsCreator = true // if you renamed, use IsOwner = true
        };
        await SendAsync(cmd);

        // Assert: requery fresh
        // TODO check if modified by is updated from 2nd update
        var demotedOwner = await FindAsync<CookbookMembership>(originalOwner.Id);
        var promotedOwner = await FindAsync<CookbookMembership>(member.Id);

        demotedOwner!.IsOwner.Should().BeFalse();
        promotedOwner!.IsOwner.Should().BeTrue();

        // Audit on the demoted membership should reflect acting user and recent timestamp
        demotedOwner.LastModifiedBy.Should().Be(actingUserId);
        demotedOwner.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(10));
    }
}
