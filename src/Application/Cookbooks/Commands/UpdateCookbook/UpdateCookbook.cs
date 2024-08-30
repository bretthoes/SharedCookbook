using SharedCookbook.Application.Common.Interfaces;

namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public record UpdateCookbookCommand : IRequest<int>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Image { get; set; }
}

public class UpdateCookbookCommandHandler : IRequestHandler<UpdateCookbookCommand, int>
{
    private readonly IApplicationDbContext _context;

    public UpdateCookbookCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Cookbooks
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        if (!string.IsNullOrWhiteSpace(request.Title)) entity.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.Image)) entity.Image = request.Image;

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
