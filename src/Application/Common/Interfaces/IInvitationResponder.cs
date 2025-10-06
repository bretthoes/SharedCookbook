using SharedCookbook.Domain.Common;
using SharedCookbook.Domain.Enums;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IInvitationResponder
{
 Task<int> Respond(BaseInvitation invite,
     InvitationStatus decision,
     CancellationToken cancellationToken);   
}
