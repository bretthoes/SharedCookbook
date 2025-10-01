namespace SharedCookbook.Application.Invitations.Commands.DeleteInvitation;

public record DeleteInvitationCommand(int Id) : IRequest;

public class DeleteInvitationCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteInvitationCommand>
{
    public async Task Handle(DeleteInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await context.CookbookInvitations.FindOrThrowAsync(command.Id, cancellationToken);

        context.CookbookInvitations.Remove(invitation);

        invitation.AddDomainEvent(new InvitationDeletedEvent(invitation));

        await context.SaveChangesAsync(cancellationToken);
    }
}

public class DeleteInvitationCommandValidator : AbstractValidator<DeleteInvitationCommand>
{
    public DeleteInvitationCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
