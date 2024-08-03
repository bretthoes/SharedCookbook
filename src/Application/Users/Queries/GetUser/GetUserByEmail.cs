
using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Users.Queries.GetUser;
public record GetUserByEmailQuery : IRequest<UserDto>
{
    public required string Email { get; set; }
}

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto>
{
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public GetUserByEmailQueryHandler(IIdentityService identityService, IMapper mapper)
    {
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(request.Email);
        return user ?? throw new NotFoundException(nameof(UserDto), request.Email);
    }
}
