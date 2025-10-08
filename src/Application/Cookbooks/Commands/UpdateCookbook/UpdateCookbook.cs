namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public sealed record UpdateCookbookCommand(int Id, string? Title = null, string? Image = null) : IRequest<int>;

public sealed class UpdateCookbookCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(request.Id, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Title))cookbook.Title = request.Title;
        if (!string.IsNullOrWhiteSpace(request.Image)) cookbook.Image = request.Image;

        cookbook.AddDomainEvent(new CookbookUpdatedEvent(cookbook));
        
        return await context.SaveChangesAsync(cancellationToken);
    }
}
