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
        ArgumentNullException.ThrowIfNull(user.Id);

        var membership = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(membership.CookbookId, user.Id, ct);

        if (command.IsOwner)
        {
            if (actorMembership is null || !actorMembership.CanPromoteToOwner(membership))
                throw new ForbiddenAccessException();

            var departingOwners = await context.CookbookMemberships
                .HasCookbookId(membership.CookbookId)
                .Where(member => member.IsOwner && member.Id != membership.Id)
                .ToListAsync(ct);

            CookbookMembership.TransferOwnershipTo(membership, departingOwners);
        }
        else
        {
            if (actorMembership is null || !actorMembership.CanUpdateMembership(membership))
                throw new ForbiddenAccessException();

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
}
