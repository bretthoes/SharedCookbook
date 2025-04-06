using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFlagColumnsForRecipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCheap",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDairyFree",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGlutenFree",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHealthy",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLowFodmap",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegan",
                table: "recipe",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegetarian",
                table: "recipe",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCheap",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsDairyFree",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsGlutenFree",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsHealthy",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsLowFodmap",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsVegan",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "IsVegetarian",
                table: "recipe");
        }
    }
}
