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

        ArgumentNullException.ThrowIfNull(token.Cookbook);
        
        string? senderId = token.CreatedBy;
        ArgumentException.ThrowIfNullOrWhiteSpace(senderId);
        
        (string? email, string? name) = await service.FindByIdAsync(senderId)
            ?? throw new NotFoundException(key: senderId, nameof(IUser));

        return new InvitationDto
        {
            Id = token.Id,
            SenderName = name,
            SenderEmail = email,
            CookbookImage = token.Cookbook?.Image,
            CookbookTitle = token.Cookbook?.Title ?? string.Empty,
        };
    }
}
