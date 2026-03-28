using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMealTypeAndProteinFlagsToRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBreakfast",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDessert",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDinner",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighProtein",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLunch",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSnack",
                table: "recipe",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBreakfast",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsDessert",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsDinner",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsHighProtein",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsLunch",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsSnack",
                table: "recipe");
        }
    }
}
