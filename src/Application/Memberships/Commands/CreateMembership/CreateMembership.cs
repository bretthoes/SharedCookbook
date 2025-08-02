namespace SharedCookbook.Application.Memberships.Commands.CreateMembership;

public record CreateMembershipCommand : IRequest
{
    public int CookbookId { get; init; }
}

public class CreateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<CreateMembershipCommand>
{
    public async Task Handle(CreateMembershipCommand command, CancellationToken token)
    {
        Guard.Against.Null(user.Id);
        
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
