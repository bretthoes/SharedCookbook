namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public sealed record UpdateMembershipCommand(Guid Id, MembershipTier Tier) : IRequest;

public sealed class UpdateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<UpdateMembershipCommand>
{
    public async Task Handle(UpdateMembershipCommand command, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var membershipToUpdate = await context.CookbookMemberships.FindOrThrowAsync(command.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(membershipToUpdate.CookbookId, user.Id, ct)
            ?? throw new ForbiddenAccessException();

        if (!actorMembership.CanApplyTierUpdate(membershipToUpdate, command.Tier))
            throw new ForbiddenAccessException();

        membershipToUpdate.SetTier(command.Tier);

        if (PromotingNewOwner(command))
            actorMembership.SetTier(MembershipTier.Contributor);

        membershipToUpdate.AddDomainEvent(new MembershipUpdatedEvent(membershipToUpdate));

        await context.SaveChangesAsync(ct);
    }

    private static bool PromotingNewOwner(UpdateMembershipCommand command) => command.Tier == MembershipTier.Owner;
}
