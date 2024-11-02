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

        _context.CookbookMembers.Add(GetNewCreatorMembership(entity));

        entity.AddDomainEvent(new CookbookCreatedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    // Creates a CookbookMember entity for the creator of a new Cookbook.
    // Contains permissions for all actions by default.
    private static CookbookMember GetNewCreatorMembership(Cookbook cookbook)
    {
        return new CookbookMember
        {
            CookbookId = cookbook.Id,
            IsCreator = true,
            CanAddRecipe = true,
            CanDeleteRecipe = true,
            CanEditCookbookDetails = true,
            CanRemoveMember = true,
            CanSendInvite = true,
            CanUpdateRecipe = true,
            Cookbook = cookbook,
        };
    }
}
