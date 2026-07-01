using SharedCookbook.Application.Contracts;

namespace SharedCookbook.Application.Common.Interfaces;

public interface IAiRecipeEditor
{
    Task<UpdateRecipeDto> ApplyEditAsync(UpdateRecipeDto current, string prompt, CancellationToken ct = default);
}
