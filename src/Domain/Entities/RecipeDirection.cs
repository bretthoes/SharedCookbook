namespace SharedCookbook.Domain.Entities;

public class RecipeDirection : BaseAuditableEntity
{
    public int RecipeId { get; set; }

    public required string Text { get; set; }

    public required int Ordinal { get; set; }

    public string? Image { get; set; }
    
    public struct Constraints
    {
        public const int TextMaxLength = 2048;
    }
}
