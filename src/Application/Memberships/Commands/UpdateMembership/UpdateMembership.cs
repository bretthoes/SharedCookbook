using SharedCookbook.Domain.ValueObjects;

namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public sealed record UpdateMembershipCommand : IRequest
{
    public required int Id { get; init; }
    public required bool IsOwner { get; init; }
    public required bool CanAddRecipe { get; init; }
    public required bool CanUpdateRecipe { get; init; }
    public required bool CanDeleteRecipe { get; init; }
    public required bool CanSendInvite { get; init; }
    public required bool CanRemoveMember { get; init; }
    public required bool CanEditCookbookDetails { get; init; }
}

public sealed class UpdateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<UpdateMembershipCommand>
{
    public async Task Handle(UpdateMembershipCommand command, CancellationToken ct = default)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);

        if (!await CanUpdateMembership(membership, ct))
            throw new ForbiddenAccessException();

        if (command.IsOwner)
        {
            var departingOwners = await context.CookbookMemberships
                .HasCookbookId(membership.CookbookId)
                .Where(member => member.IsOwner && member.Id != membership.Id)
                .ToListAsync(ct);

            CookbookMembership.TransferOwnershipTo(membership, departingOwners);
        }
        else
        {
            membership.SetPermissions(membership.Permissions
                .WithAddRecipe(command.CanAddRecipe)
                .WithUpdateRecipe(command.CanUpdateRecipe)
                .WithDeleteRecipe(command.CanDeleteRecipe)
                .WithSendInvite(command.CanSendInvite)
                .WithRemoveMember(command.CanRemoveMember)
                .WithEditCookbookDetails(command.CanEditCookbookDetails));
        }

        membership.AddDomainEvent(new MembershipUpdatedEvent(membership));
        
        await context.SaveChangesAsync(ct);
    }

    private async Task<bool> CanUpdateMembership(CookbookMembership membership, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return false;

        if (string.Equals(membership.CreatedBy, user.Id, StringComparison.Ordinal))
            return false;

        var actor = await context.CookbookMemberships.FindForUserAsync(membership.CookbookId, user.Id, ct);

        return actor is not null && actor.IsOwner;
    }
}
