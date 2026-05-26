using Microsoft.Extensions.Options;
using SharedCookbook.Application.Images.Commands.CreateImages;

namespace SharedCookbook.Application.Cookbooks.Commands.UpdateCookbook;

public sealed record UpdateCookbookCommand(int Id, string? Title = null, string? Image = null) : IRequest<int>;

public sealed class UpdateCookbookCommandHandler(
    IApplicationDbContext context,
    IOptions<ImageUploadOptions> options,
    IUser user)
    : IRequestHandler<UpdateCookbookCommand, int>
{
    public async Task<int> Handle(UpdateCookbookCommand request, CancellationToken ct = default)
    {
        var cookbook = await context.Cookbooks.FindOrThrowAsync(request.Id, ct);

        if (!await CanUpdateCookbook(cookbook.Id, ct))
            throw new ForbiddenAccessException();

        cookbook.Title = request.Title ?? string.Empty;
        cookbook.Image = request.Image?.StripPrefixUrl(options.Value.ImageBaseUrl);

        cookbook.AddDomainEvent(new CookbookUpdatedEvent(cookbook));
        
        return await context.SaveChangesAsync(ct);
    }

    private async Task<bool> CanUpdateCookbook(int cookbookId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
            return false;

        var actor = await context.CookbookMemberships.FindForUserAsync(cookbookId, user.Id, ct);

        return actor is not null && actor.Permissions.CanEditCookbookDetails;
    }
}
