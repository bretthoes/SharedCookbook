using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.InvitationTokens.Commands.UpdateInvitationToken;

public sealed record UpdateInvitationTokenCommand(string Token, InvitationStatus NewStatus) : IRequest<int>;

public sealed class UpdateInvitationTokenCommandHandler(
    IApplicationDbContext context, 
    IInvitationTokenFactory factory,
    IInvitationResponder responder)
    : IRequestHandler<UpdateInvitationTokenCommand, int>
{
    public async Task<int> Handle(UpdateInvitationTokenCommand command, CancellationToken cancellationToken)
    {
        var link = TokenLink.Parse(command.Token);
        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken)
                    ?? throw new NotFoundException(key: link.TokenId.ToString(), nameof(InvitationToken));
        
        if (!factory.Verify(link.Secret, token.Digest)) throw new TokenDigestMismatchException();
        if (!token.IsRedeemable) throw new TokenIsNotRedeemableException();
        
        return await responder.Respond(token, command.NewStatus, cancellationToken);
    }
}

public class UpdateInvitationTokenCommandValidator : AbstractValidator<UpdateInvitationTokenCommand>
{
    public UpdateInvitationTokenCommandValidator()
    {
        RuleFor(command => command.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
