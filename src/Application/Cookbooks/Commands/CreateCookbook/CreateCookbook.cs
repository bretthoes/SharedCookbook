using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public record CreateCookbookCommand : IRequest<int>
{
    public required string Title { get; set; }

    public string? Image { get; set; }
}

public class CreateCookbookCommandHandler : IRequestHandler<CreateCookbookCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public CreateCookbookCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        var entity = new Cookbook
        {
            Title = request.Title,
            Image = request.Image,
        };

        _context.Cookbooks.Add(entity);

        _context.CookbookMembers.Add(CookbookMember.GetNewCreatorMembership(entity.Id));

        entity.AddDomainEvent(new CookbookCreatedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
