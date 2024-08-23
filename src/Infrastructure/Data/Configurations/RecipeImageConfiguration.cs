using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeImageConfiguration : IEntityTypeConfiguration<RecipeImage>
{
    public void Configure(EntityTypeBuilder<RecipeImage> builder)
    {
        builder.ToTable("recipe_image");

        builder.HasKey(ri => ri.Id)
            .HasName("PK_recipe_image_id");

        builder.HasIndex(ri => ri.RecipeId, "IX_recipe_image__recipe_id");

        builder.Property(ri => ri.Id)
            .HasColumnName("recipe_image_id")
            .IsRequired();

        builder.Property(ri => ri.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();

        builder.Property(ri => ri.ImageUrl)
            .HasMaxLength(2048)
            .HasColumnName("image_url")
            .IsRequired();

        builder.Property(ri => ri.Ordinal)
            .HasColumnName("ordinal")
            .IsRequired();

        builder.HasOne<Recipe>()
            .WithMany(r => r.Images)
            .HasForeignKey(ri => ri.RecipeId)
            .HasConstraintName("FK_recipe_image__recipe_id");
    }
}
