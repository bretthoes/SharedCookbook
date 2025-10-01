namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public record UpdateCookbookCommand(int Id, string? Title = null, string? Image = null) : IRequest<int>;

public class UpdateCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks .FindAsync(keyValues: [request.Id], cancellationToken)
            ?? throw new NotFoundException(key: request.Id.ToString(), nameof(Cookbook));

        if (!string.IsNullOrWhiteSpace(request.Title)) cookbook.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.Image)) cookbook.Image = request.Image;

        return await context.SaveChangesAsync(cancellationToken);
    }
}
