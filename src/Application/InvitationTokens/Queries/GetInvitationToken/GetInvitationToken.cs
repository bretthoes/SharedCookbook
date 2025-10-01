using SharedCookbook.Application.Common.Extensions;
using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;

namespace SharedCookbook.Application.InvitationTokens.Queries.GetInvitationToken;

public record GetInvitationTokenQuery(string Token) : IRequest<InvitationDto>;

public class GetInvitationPreviewQueryHandler(
    IApplicationDbContext context,
    IIdentityService service)
    : IRequestHandler<GetInvitationTokenQuery, InvitationDto>
{
    public async Task<InvitationDto> Handle(GetInvitationTokenQuery query, CancellationToken cancellationToken)
    {
        var link = TokenLink.Parse(query.Token);

        var token = await context.InvitationTokens.SingleById(link.TokenId, cancellationToken)
            ?? throw new NotFoundException(key: link.TokenId.ToString(), nameof(cancellationToken));

        var cookbook = token.Cookbook;
        ArgumentNullException.ThrowIfNull(cookbook);
        
        string? senderId = token.CreatedBy;
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId);
        
        var userDto = await service.FindByIdAsync(senderId)
            ?? throw new NotFoundException(key: senderId, nameof(UserDto));

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

public class GetInvitationQueryValidator : AbstractValidator<GetInvitationTokenQuery>
{
    public GetInvitationQueryValidator()
    {
        RuleFor(query => query.Token)
            .NotEmpty().WithMessage("Token cannot be empty.");
    }
}
