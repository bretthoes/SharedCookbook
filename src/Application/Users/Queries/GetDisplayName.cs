namespace SharedCookbook.Application.Users.Queries;

public record GetDisplayNameQuery : IRequest<string>;

public class GetDisplayNameQueryHandler(IUser user, IIdentityService identityService)
    : IRequestHandler<GetDisplayNameQuery, string>
{
    public async Task<string> Handle(GetDisplayNameQuery query, CancellationToken cancellationToken)
        => await identityService
            .GetDisplayNameAsync(user.Id ?? throw new UnauthorizedAccessException()) ?? "";
}
