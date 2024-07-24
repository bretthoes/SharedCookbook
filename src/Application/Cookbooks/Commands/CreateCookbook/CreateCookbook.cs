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

        //entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        _context.Cookbooks.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
