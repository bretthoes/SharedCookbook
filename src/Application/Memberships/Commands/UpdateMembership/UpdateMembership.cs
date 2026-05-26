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
        if (string.IsNullOrWhiteSpace(user.Id))
            throw new ForbiddenAccessException();

        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);

        var actor = await context.CookbookMemberships.FindForUserAsync(membership.CookbookId, user.Id, ct);

        if (actor is null || !CanUpdateMembership(membership, actor))
            throw new ForbiddenAccessException();

        if (command.IsOwner)
        {
            // Only the current owner can promote another member to owner
            if (!actor.IsOwner)
                throw new ForbiddenAccessException();

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

    private static bool CanUpdateMembership(CookbookMembership membership, CookbookMembership actor)
    {
        if (string.Equals(membership.CreatedBy, actor.CreatedBy, StringComparison.Ordinal))
            return false;

        return actor.IsOwner || actor.Permissions.CanRemoveMember;
    }
}
