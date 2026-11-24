using SharedCookbook.Application.Memberships.Queries.FindForUser;

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

        if (!actorMembership.TryApplyTierUpdate(membershipToUpdate, command.Tier))
            throw new ForbiddenAccessException();

        await context.SaveChangesAsync(ct);
    }
}
