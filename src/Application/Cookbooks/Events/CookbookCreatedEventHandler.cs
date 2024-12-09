using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Cookbooks.Events;

public class CookbookCreatedEventHandler : INotificationHandler<CookbookCreatedEvent>
{
    private readonly IApplicationDbContext _context;

    public CookbookCreatedEventHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
    }

    public async Task Handle(CookbookCreatedEvent notification, CancellationToken cancellationToken)
    {
        var cookbook = notification.Cookbook;

        var creatorMembership = CookbookMember.GetNewCreatorMembership(cookbook.Id);

        _context.CookbookMembers.Add(creatorMembership);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
