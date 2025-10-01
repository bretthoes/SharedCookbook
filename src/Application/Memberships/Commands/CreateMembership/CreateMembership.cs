namespace SharedCookbook.Application.Memberships.Commands.CreateMembership;

public record CreateMembershipCommand(int CookbookId) : IRequest;

public class CreateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<CreateMembershipCommand>
{
    public async Task Handle(CreateMembershipCommand command, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        if (await MembershipAlreadyExistsInCookbook(command.CookbookId, user.Id, token)) return;
        
        var membership = CookbookMembership.GetDefaultMembership(command.CookbookId);

        await context.CookbookMemberships.AddAsync(membership, token);
        await context.SaveChangesAsync(token);
    }
    
    private async Task<bool> MembershipAlreadyExistsInCookbook(
            int cookbookId,
            string userId,
            CancellationToken token) 
            => await context.CookbookMemberships
                .AnyAsync(membership
                    => membership.CookbookId == cookbookId
                       && membership.CreatedBy == userId, token);
}

public class CreateMembershipCommandValidator : AbstractValidator<CreateMembershipCommand>
{
    public CreateMembershipCommandValidator()
    {
        RuleFor(command => command.CookbookId)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
