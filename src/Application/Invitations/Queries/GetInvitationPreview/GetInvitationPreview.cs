using SharedCookbook.Application.Common.Exceptions;
using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Application.Invitations.Queries.GetInvitationPreview;

public record GetInvitationPreviewQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context,
    IIdentityService service,
    IInvitationTokenFactory factory)
    : IRequestHandler<GetInvitationPreviewQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationPreviewQuery query, CancellationToken cancellationToken)
    {
        var link = TokenLink.Parse(query.Token);

        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken);
        
        
        var cookbook = token.Cookbook;
        ArgumentNullException.ThrowIfNull(cookbook);
        
        string? senderId = token.CreatedBy;
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId);
        
        Throw.IfFalse<TokenDigestMismatchException>(factory.Verify(query.Token, token.Digest));
        Throw.IfFalse<TokenIsNotConsumableException>(token.IsActive);
        
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

public class GetInvitationPreviewQueryValidator : AbstractValidator<GetInvitationPreviewQuery>
{
    public GetInvitationPreviewQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
