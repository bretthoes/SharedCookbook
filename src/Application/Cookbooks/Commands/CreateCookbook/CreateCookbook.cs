using SharedCookbook.Application.Common.Interfaces;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Domain.Events;

namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public record CreateCookbookCommand : IRequest<int>
{
    public int CreatorPersonId { get; set; }

    public required string Title { get; set; }

    public required string ImagePath { get; set; }
}

public class CreateCookbookCommandHandler : IRequestHandler<CreateCookbookCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCookbookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        var entity = new Cookbook
        {
            CreatorPersonId = request.CreatorPersonId,
            Title = request.Title,
            ImagePath = request.ImagePath,
        };

        _context.Cookbooks.Add(entity);

        AddCookbookCreator(request, entity);

        //entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    // Creates a CookbookMember entity for the creator of a new Cookbook.
    // Contains permissions for all actions by default.
    private void AddCookbookCreator(CreateCookbookCommand request, Cookbook cookbook)
    {
        var creatorMembership = new CookbookMember
        {
            PersonId = request.CreatorPersonId,
            CookbookId = cookbook.Id,
            CanAddRecipe = true,
            CanDeleteRecipe = true,
            CanEditCookbookDetails = true,
            CanRemoveMember = true,
            CanSendInvite = true,
            CanUpdateRecipe = true,
            Cookbook = cookbook
        };

        _context.CookbookMembers.Add(creatorMembership);
    }
}
