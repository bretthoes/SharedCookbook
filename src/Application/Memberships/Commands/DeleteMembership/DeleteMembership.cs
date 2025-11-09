namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;
public class DeleteMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteMembershipCommand>
{
    public async Task Handle(DeleteMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await context.CookbookMemberships.FindOrThrowAsync(request.Id, cancellationToken);

        context.CookbookMemberships.Remove(membership);
        membership.AddDomainEvent(new MembershipDeletedEvent(membership));

        await context.SaveChangesAsync(cancellationToken);
    }
}
