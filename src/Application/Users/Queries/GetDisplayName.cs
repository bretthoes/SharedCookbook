namespace SharedCookbook.Application.Users.Queries;

public record GetDisplayNameQuery : IRequest<DisplayNameDto>;

public class GetDisplayNameQueryHandler(IUser user, IIdentityService identityService)
    : IRequestHandler<GetDisplayNameQuery, DisplayNameDto>
{
    public async Task<DisplayNameDto> Handle(GetDisplayNameQuery query, CancellationToken cancellationToken)
        => new(await identityService.GetDisplayNameAsync(user.Id ?? throw new UnauthorizedAccessException()) ?? "");
}
