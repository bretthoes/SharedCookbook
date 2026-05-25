namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;
public class DeleteMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteMembershipCommand>
{
    public async Task Handle(DeleteMembershipCommand request, CancellationToken ct = default)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(request.Id, ct);

        context.CookbookMemberships.Remove(membership);
        membership.AddDomainEvent(new MembershipDeletedEvent(membership));

        await context.SaveChangesAsync(ct);
    }
}
