using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeNutritionConfiguration : IEntityTypeConfiguration<RecipeNutrition>
{
    public void Configure(EntityTypeBuilder<RecipeNutrition> builder)
    {
        builder.HasKey(rn => rn.Id)
        .HasName("PK_recipe_nutrition_id");

        builder.ToTable("recipe_nutrition");

        builder.HasIndex(
            rn => rn.RecipeId,
            "IX_recipe_nutrition__recipe_id");

        builder.Property(rn => rn.Id)
            .HasColumnName("recipe_nutrition_id")
            .IsRequired();
        builder.Property(rn => rn.RecipeId)
            .HasColumnName("recipe_id");
        builder.Property(rn => rn.Calories)
            .HasColumnName("calories");
        builder.Property(rn => rn.Protein)
            .HasColumnName("protein");
        builder.Property(rn => rn.Fat)
            .HasColumnName("fat");
        builder.Property(rn => rn.Carbohydrates)
            .HasColumnName("carbohydrates");
        builder.Property(rn => rn.Sugar)
            .HasColumnName("sugar");
        builder.Property(rn => rn.Fiber)
            .HasColumnName("fiber");
        builder.Property(rn => rn.Sodium)
            .HasColumnName("sodium");

        builder.HasOne<Recipe>()
        .WithOne(r => r.Nutrition)
        .HasForeignKey<RecipeNutrition>(rn => rn.RecipeId)
        .HasConstraintName("FK_recipe_nutrition__recipe_id");
    }
}
