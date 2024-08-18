
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Users.Queries.GetUser;
public record GetUserQuery : IRequest<UserDto>
{
    public required int Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IIdentityService identityService, IMapper mapper)
    {
        _identityService = identityService;
        _mapper = mapper;
    }

    // TODO why are we mapping in the service and not here? add comment if valid reason
    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByIdAsync(request.Id.ToString());

        Guard.Against.NotFound(request.Id, user);

        return user;
    }
}
