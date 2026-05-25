using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SharedCookbook.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(string DisplayName) : IRequest;

public class UpdateUserCommandHandler(IIdentityService service, IUser user) 
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        var result = await service.UpdateUserAsync(user.Id, request.DisplayName.Trim(), ct);

        if (!result.Succeeded) throw new ValidationException("Could not update user.");
    }
}
