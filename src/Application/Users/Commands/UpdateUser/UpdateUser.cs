using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SharedCookbook.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(string DisplayName) : IRequest;

public class UpdateUserCommandHandler(IIdentityService service, IUser user) 
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);
        
        var result = await service.UpdateUserAsync(user.Id, request.DisplayName.Trim());

        if (!result.Succeeded) throw new ValidationException("Could not update user.");
    }
}
