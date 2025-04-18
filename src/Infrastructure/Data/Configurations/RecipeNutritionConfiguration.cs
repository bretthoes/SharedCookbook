using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeNutritionConfiguration : IEntityTypeConfiguration<RecipeNutrition>
{
    public void Configure(EntityTypeBuilder<RecipeNutrition> builder)
    {
        builder.HasKey(recipeNutrition => recipeNutrition.Id)
        .HasName("PK_recipe_nutrition_id");

        builder.ToTable("recipe_nutrition");

        builder.HasIndex(
            recipeNutrition => recipeNutrition.RecipeId,
            name: "IX_recipe_nutrition__recipe_id");

        builder.Property(recipeNutrition => recipeNutrition.Id)
            .HasColumnName("recipe_nutrition_id")
            .IsRequired();
        builder.Property(recipeNutrition => recipeNutrition.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(recipeNutrition => recipeNutrition.Calories)
            .HasColumnName("calories");
        builder.Property(recipeNutrition => recipeNutrition.Protein)
            .HasColumnName("protein");
        builder.Property(recipeNutrition => recipeNutrition.Fat)
            .HasColumnName("fat");
        builder.Property(recipeNutrition => recipeNutrition.Carbohydrates)
            .HasColumnName("carbohydrates");
        builder.Property(recipeNutrition => recipeNutrition.Sugar)
            .HasColumnName("sugar");
        builder.Property(recipeNutrition => recipeNutrition.Fiber)
            .HasColumnName("fiber");
        builder.Property(recipeNutrition => recipeNutrition.Sodium)
            .HasColumnName("sodium");

        builder.HasOne<Recipe>()
        .WithOne(recipe => recipe.Nutrition)
        .HasForeignKey<RecipeNutrition>(recipeNutrition => recipeNutrition.RecipeId)
        .HasConstraintName("FK_recipe_nutrition__recipe_id");
    }
}
