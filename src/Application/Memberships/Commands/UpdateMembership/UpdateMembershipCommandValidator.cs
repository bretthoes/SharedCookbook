namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public class UpdateMembershipCommandValidator : AbstractValidator<UpdateMembershipCommand>
{
    public UpdateMembershipCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Tier).IsInEnum();
    }
}
