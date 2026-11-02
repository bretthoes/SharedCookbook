namespace SharedCookbook.Application.Common.Interfaces;

public interface IAiRecipeEditor
{
    Task<RecipeDto> ApplyEditAsync(RecipeDto current, string prompt, CancellationToken ct = default);
}
