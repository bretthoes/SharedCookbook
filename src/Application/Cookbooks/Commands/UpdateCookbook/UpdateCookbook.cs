namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public record UpdateCookbookCommand : IRequest<int>
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Image { get; init; }
}

public class UpdateCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks
            .FindAsync(keyValues: [request.Id], cancellationToken);

        Guard.Against.NotFound(request.Id, cookbook);

        if (!string.IsNullOrWhiteSpace(request.Title)) cookbook.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.Image)) cookbook.Image = request.Image;

        return await context.SaveChangesAsync(cancellationToken);
    }
}
