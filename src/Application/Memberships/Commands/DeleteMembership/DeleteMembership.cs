namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;

public class DeleteMembershipCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<DeleteMembershipCommand>
{
    public async Task Handle(DeleteMembershipCommand request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(user.Id);

        var membership = await context.CookbookMemberships.FindOrThrowAsync(request.Id, ct);
        var actorMembership = await context.CookbookMemberships.FindForUserAsync(membership.CookbookId, user.Id, ct);

        if (actorMembership is null || !actorMembership.CanRemoveMember(membership))
            throw new ForbiddenAccessException();

        context.CookbookMemberships.Remove(membership);
        membership.AddDomainEvent(new MembershipDeletedEvent(membership));

        await context.SaveChangesAsync(ct);
    }
}
