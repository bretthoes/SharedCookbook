using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public sealed record UpdateCookbookCommand(int Id, string? Title = null, string? Image = null) : IRequest<int>;

public sealed class UpdateCookbookCommandHandler(IApplicationDbContext context, IOptions<ImageUploadOptions> options)
    : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken cancellationToken)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(request.Id, cancellationToken);

        cookbook.Title = request.Title ?? string.Empty;
        cookbook.Image = request.Image?.StripPrefixUrl(options.Value.ImageBaseUrl);

        cookbook.AddDomainEvent(new CookbookUpdatedEvent(cookbook));
        
        return await context.SaveChangesAsync(cancellationToken);
    }
}
