namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public record DeleteMembershipCommand(int Id) : IRequest;
public class DeleteMembershipCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteMembershipCommand>
{
    public async Task Handle(DeleteMembershipCommand request, CancellationToken cancellationToken)
    {
        var membership = await context.CookbookMemberships
            .FindAsync(keyValues: [request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, membership);

        context.CookbookMemberships.Remove(membership);
        membership.MarkDeleted();

        await context.SaveChangesAsync(cancellationToken);
    }
}

public class DeleteMembershipCommandValidator : AbstractValidator<DeleteMembershipCommand>
{
    public DeleteMembershipCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
