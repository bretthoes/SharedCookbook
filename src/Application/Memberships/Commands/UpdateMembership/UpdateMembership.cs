namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public sealed record UpdateMembershipCommand(int Id, MembershipTier Tier) : IRequest;

public sealed class UpdateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<UpdateMembershipCommand>
{
    public async Task Handle(UpdateMembershipCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var membershipToUpdate = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(membershipToUpdate.CookbookId, user.Id, ct)
            ?? throw new ForbiddenAccessException();

        if (IsTransferringOwnership(command))
            HandleTransferOwnership(currentOwner: actorMembership, successor: membershipToUpdate);
        else
        {
            if (!actorMembership.CanApplyTierUpdate(membershipToUpdate, command.Tier))
                throw new ForbiddenAccessException();

            membershipToUpdate.SetTier(command.Tier);
            membershipToUpdate.AddDomainEvent(new MembershipUpdatedEvent(membershipToUpdate));
        }

        await context.SaveChangesAsync(ct);
    }
    
    private static bool IsTransferringOwnership(UpdateMembershipCommand command) => command.Tier == MembershipTier.Owner;

    private static void HandleTransferOwnership(CookbookMembership currentOwner, CookbookMembership successor)
    {
        if (!currentOwner.CanPromoteToOwner(successor)) throw new ForbiddenAccessException();

        currentOwner.Demote();
        successor.Promote();
    }
}
