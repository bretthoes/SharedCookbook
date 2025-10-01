using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

public record GetInvitationTokenQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context,
    IIdentityService service,
    IInvitationTokenFactory factory)
    : IRequestHandler<GetInvitationTokenQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationTokenQuery query, CancellationToken cancellationToken)
    {
        var link = TokenLink.Parse(query.Token);

        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken);
        
        var cookbook = token.Cookbook;
        ArgumentNullException.ThrowIfNull(cookbook);
        
        string? senderId = token.CreatedBy;
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId);
        
        Throw.IfFalse<TokenDigestMismatchException>(factory.Verify(link.Secret, token.Digest));
        Throw.IfFalse<TokenIsNotConsumableException>(token.IsRedeemable);
        
        var userDto = await service.FindByIdAsync(senderId);
        Guard.Against.NotFound(senderId, userDto);

        return new InvitationDto
        {
            Id = token.Id,
            SenderName = userDto.DisplayName,
            SenderEmail = userDto.Email,
            CookbookImage = token.Cookbook?.Image,
            CookbookTitle = token.Cookbook?.Title ?? string.Empty,
        };
    }
}

public class GetInvitationPreviewQueryValidator : AbstractValidator<GetInvitationTokenQuery>
{
    public GetInvitationPreviewQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
