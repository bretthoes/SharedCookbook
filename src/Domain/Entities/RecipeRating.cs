namespace SharedCookbook.Domain.Entities;

public class RecipeRating : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public required int PersonId { get; set; }

    public required int RatingValue { get; set; }
}
