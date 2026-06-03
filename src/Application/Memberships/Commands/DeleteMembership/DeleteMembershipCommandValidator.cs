namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public class DeleteMembershipCommandValidator : AbstractValidator<DeleteMembershipCommand>
{
    public DeleteMembershipCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty();
    }
}
