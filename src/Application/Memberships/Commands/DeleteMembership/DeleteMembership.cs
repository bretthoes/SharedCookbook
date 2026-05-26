namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;

public class DeleteMembershipCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteMembershipCommand>
{
    public async Task Handle(DeleteMembershipCommand request, CancellationToken ct = default)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(request.Id, ct);

        if (!await CanDeleteMembership(membership, ct))
            throw new ForbiddenAccessException();

        context.CookbookMemberships.Remove(membership);
        membership.AddDomainEvent(new MembershipDeletedEvent(membership));

        await context.SaveChangesAsync(ct);
    }

    private async Task<bool> CanDeleteMembership(CookbookMembership membership, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return false;

        if (string.Equals(membership.CreatedBy, user.Id, StringComparison.Ordinal))
            return true;

        var actor = await context.CookbookMemberships.FindForUserAsync(membership.CookbookId, user.Id, ct);

        return actor is not null && actor.Permissions.CanRemoveMember;
    }
}
