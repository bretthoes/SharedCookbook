using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class IngredientSectionConfiguration : IEntityTypeConfiguration<IngredientSection>
{
    public void Configure(EntityTypeBuilder<IngredientSection> builder)
    {
        builder.ToTable("ingredient_section");

        builder.HasKey(section => section.Id)
            .HasName("PK_ingredient_section_id");

        builder.HasIndex(
            section => section.RecipeId,
            name: "IX_ingredient_section__recipe_id");

        builder.Property(section => section.Id)
            .HasColumnName("ingredient_section_id")
            .IsRequired();
        builder.Property(section => section.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(section => section.Title)
            .HasMaxLength(IngredientSection.Constraints.TitleMaxLength)
            .HasColumnName("title")
            .IsRequired();
        builder.Property(section => section.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany(recipe => recipe.IngredientSections)
            .HasForeignKey(section => section.RecipeId)
            .HasConstraintName("FK_ingredient_section__recipe_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
