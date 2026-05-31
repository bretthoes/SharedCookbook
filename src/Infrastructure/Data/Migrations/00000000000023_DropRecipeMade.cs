using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class DropRecipeMade : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "recipe_made");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recipe_made",
            columns: table => new
            {
                recipe_made_id = table.Column<int>(type: "integer", nullable: false),
                recipe_id = table.Column<int>(type: "integer", nullable: false),
                user_id = table.Column<string>(type: "text", nullable: false),
                made_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                made_on_utc_date = table.Column<DateOnly>(type: "date", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recipe_made_id", x => x.recipe_made_id);
                table.ForeignKey(
                    name: "FK_recipe_made__recipe_id",
                    column: x => x.recipe_id,
                    principalTable: "recipe",
                    principalColumn: "recipe_id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_recipe_made__user_id",
                    column: x => x.user_id,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_recipe_made__recipe_id",
            table: "recipe_made",
            column: "recipe_id");

        migrationBuilder.CreateIndex(
            name: "UX_recipe_made__user_recipe_day",
            table: "recipe_made",
            columns: new[] { "user_id", "recipe_id", "made_on_utc_date" },
            unique: true);
    }
}
