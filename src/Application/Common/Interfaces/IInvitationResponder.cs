using SharedCookbook.Domain.Common;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IInvitationResponder
{
 Task<Guid> Respond(BaseInvitation invite,
     InvitationStatus decision,
     CancellationToken ct = default);   
}
