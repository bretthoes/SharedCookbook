using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedCookbook.Domain.Entities;
using SharedCookbook.Infrastructure.Identity;

namespace SharedCookbook.Infrastructure.Data.Configurations;

public class RecipeRatingConfiguration : IEntityTypeConfiguration<RecipeRating>
{
    public void Configure(EntityTypeBuilder<RecipeRating> builder)
    {
        builder.ToTable("recipe_rating");

        builder.HasKey(rr => rr.Id)
            .HasName("PK_recipe_rating_id");

        builder.HasIndex(
            rr => rr.PersonId,
            "IX_recipe_rating__person_id");
        builder.HasIndex(
            rr => rr.RecipeId,
            "IX_recipe_rating__recipe_id");

        builder.Property(rr => rr.Id)
            .HasColumnName("recipe_rating_id")
            .IsRequired();
        builder.Property(rr => rr.RecipeId)
            .HasColumnName("recipe_id")
            .IsRequired();
        builder.Property(rr => rr.PersonId)
            .HasColumnName("person_id")
            .IsRequired();
        builder.Property(rr => rr.RatingValue)
            .HasColumnName("rating_value")
            .IsRequired();
        builder.Property(rr => rr.Created)
            .HasColumnName("created")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithMany(p => p.RecipeRatings)
            .HasForeignKey(rr => rr.PersonId)
            .HasConstraintName("FK_recipe_rating__person_id");
        builder.HasOne<Recipe>()
            .WithMany(p => p.RecipeRatings)
            .HasForeignKey(rr => rr.RecipeId)
            .HasConstraintName("FK_recipe_rating__recipe_id");
    }
}
