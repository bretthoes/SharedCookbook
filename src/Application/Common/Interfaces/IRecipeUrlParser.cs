namespace SharedCookbook.Application.Common.Interfaces;

public interface IRecipeUrlParser
{
    Task<CreateRecipeDto> Parse(string url, bool extractFromVideo = false, CancellationToken ct = default);
}
