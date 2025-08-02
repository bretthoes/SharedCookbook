namespace SharedCookbook.Application.Cookbooks.Commands.CreateCookbook;

public record CreateCookbookCommand : IRequest<int>
{
    public required string Title { get; init; }

    public string? Image { get; init; }
}

public class CreateCookbookCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateCookbookCommand, int>
{
    public async Task<int> Handle(CreateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = Cookbook.Create(request.Title, request.Image);
        
        await context.Cookbooks.AddAsync(cookbook, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return cookbook.Id;
    }
}
