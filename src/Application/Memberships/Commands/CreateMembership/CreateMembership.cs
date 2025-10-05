using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;

namespace SharedCookbook.Application.Memberships.Commands.CreateMembership;

public record CreateMembershipCommand(int CookbookId) : IRequest;

public class CreateMembershipCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<CreateMembershipCommand>
{
    public async Task Handle(CreateMembershipCommand command, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        if (await context.CookbookMemberships.IsMember(command.CookbookId, user.Id, token))
            throw new MembershipAlreadyExistsException(command.CookbookId, user.Id);
        
        await context.CookbookMemberships.AddAsync(CookbookMembership.NewDefault(command.CookbookId), token);
        await context.SaveChangesAsync(token);
    }
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
