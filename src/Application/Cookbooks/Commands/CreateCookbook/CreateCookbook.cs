namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public sealed record CreateCookbookCommand(string Title, string? Image = null) : IRequest<int>;

public sealed class CreateCookbookCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateCookbookCommand, int>
{
    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = Cookbook.Create(request.Title, request.Image);
        
        await context.Cookbooks.AddAsync(cookbook, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return cookbook.Id;
    }
}
