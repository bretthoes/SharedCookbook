namespace SharedCookbook.Application.Memberships.Commands.UpdateMembership;

public class UpdateMembershipCommandValidator : AbstractValidator<UpdateMembershipCommand>
{
    public UpdateMembershipCommandValidator()
    {
        RuleFor(command => command.Id).NotNull().GreaterThan(1);
        RuleFor(command => command.IsOwner).NotNull();
        RuleFor(command => command.CanAddRecipe).NotNull();
        RuleFor(command => command.CanUpdateRecipe).NotNull();
        RuleFor(command => command.CanDeleteRecipe).NotNull();
        RuleFor(command => command.CanSendInvite).NotNull();
        RuleFor(command => command.CanRemoveMember).NotNull();
        RuleFor(command => command.CanEditCookbookDetails).NotNull();
    }
}
