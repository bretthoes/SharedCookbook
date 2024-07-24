namespace SharedCookbook.Domain.Entities;

public class RecipeNutrition : BaseAuditableEntity
{
    public required int RecipeId { get; set; }

    public int? Calories { get; set; }

    public int? Protein { get; set; }

    public int? Fat { get; set; }

    public int? Carbohydrates { get; set; }

    public int? Sugar { get; set; }

    public int? Fiber { get; set; }

    public int? Sodium { get; set; }
}
