
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

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByIdAsync(request.Id.ToString());
        return user ?? throw new NotFoundException(nameof(UserDto), request.Id.ToString());
    }
}
