namespace SharedCookbook.Domain.Events;

public class PromotedToOwnerEvent(int membershipId, int cookbookId) : BaseEvent
{
    public int MembershipId { get; } =  membershipId;
    public int CookbookId { get; } = cookbookId;
}
