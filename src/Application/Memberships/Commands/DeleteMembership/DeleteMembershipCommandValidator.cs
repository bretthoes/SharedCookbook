namespace SharedCookbook.Application.Memberships.Commands.DeleteMembership;

public class DeleteMembershipCommandValidator : AbstractValidator<DeleteMembershipCommand>
{
    public DeleteMembershipCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
