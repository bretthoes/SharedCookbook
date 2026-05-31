using SharedCookbook.Domain.Enums;

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

        if (command.Tier == MembershipTier.Owner)
        {
            if (!actorMembership.CanPromoteToOwner(membershipToUpdate))
                throw new ForbiddenAccessException();

            membershipToUpdate.Promote();
        }
        else
        {
            if (!actorMembership.CanApplyTierUpdate(membershipToUpdate, command.Tier))
                throw new ForbiddenAccessException();

            membershipToUpdate.SetTier(command.Tier);
            membershipToUpdate.AddDomainEvent(new MembershipUpdatedEvent(membershipToUpdate));
        }

        await context.SaveChangesAsync(ct);
    }
}
