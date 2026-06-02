using SharedCookbook.Application.Common.Performance;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace SharedCookbook.Application.Users.Commands.UpdateUser;

[LongRunningRequest(LongRunningRequestCategory.DataPropagation)]
public record UpdateUserCommand(string DisplayName) : IRequest;

public class UpdateUserCommandHandler(
    IIdentityService service,
    IDisplayNamePropagation propagation,
    IUser user)
    : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user.Id);

        var displayName = request.DisplayName.Trim();
        var result = await service.UpdateUserAsync(user.Id, displayName, ct);

        if (!result.Succeeded) throw new ValidationException("Could not update user.");

        // TODO move to domain event handler??
        await propagation.PropagateAsync(user.Id, displayName, ct);
    }
}
