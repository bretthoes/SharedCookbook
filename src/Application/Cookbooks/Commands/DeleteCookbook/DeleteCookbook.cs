using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Cookbooks.Commands.DeleteCookbook;

public record DeleteCookbookCommand(int Id) : IRequest;
public class DeleteCookbookCommandHandler : IRequestHandler<DeleteCookbookCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteCookbookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCookbookCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Cookbooks
            .FindAsync([request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        _context.Cookbooks.Remove(entity);

        //entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        await _context.SaveChangesAsync(cancellationToken);
    }
}
