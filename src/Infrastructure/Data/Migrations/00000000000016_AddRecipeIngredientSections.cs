using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeIngredientSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ingredient_section",
                columns: table => new
                {
                    ingredient_section_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ingredient_section_id", x => x.ingredient_section_id);
                    table.ForeignKey(
                        name: "FK_ingredient_section__recipe_id",
                        column: x => x.recipe_id,
                        principalTable: "recipe",
                        principalColumn: "recipe_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ingredient_section__recipe_id",
                table: "ingredient_section",
                column: "recipe_id");

            migrationBuilder.AddColumn<int>(
                name: "ingredient_section_id",
                table: "recipe_ingredient",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                INSERT INTO ingredient_section (recipe_id, title, ordinal, "Created", "CreatedBy", "LastModified", "LastModifiedBy")
                SELECT ri.recipe_id, '', 0, MIN(ri."Created"), NULL, MAX(ri."LastModified"), NULL
                FROM recipe_ingredient ri
                GROUP BY ri.recipe_id;
                """);

            migrationBuilder.Sql(
                """
                UPDATE recipe_ingredient ri
                SET ingredient_section_id = s.ingredient_section_id
                FROM ingredient_section s
                WHERE s.recipe_id = ri.recipe_id;
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_recipe_ingredient__recipe_id",
                table: "recipe_ingredient");

            migrationBuilder.DropIndex(
                name: "IX_recipe_ingredient__recipe_id",
                table: "recipe_ingredient");

            migrationBuilder.DropColumn(
                name: "recipe_id",
                table: "recipe_ingredient");

            migrationBuilder.AlterColumn<int>(
                name: "ingredient_section_id",
                table: "recipe_ingredient",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_ingredient__ingredient_section_id",
                table: "recipe_ingredient",
                column: "ingredient_section_id",
                principalTable: "ingredient_section",
                principalColumn: "ingredient_section_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipe_ingredient__ingredient_section_id",
                table: "recipe_ingredient");

            migrationBuilder.AddColumn<int>(
                name: "recipe_id",
                table: "recipe_ingredient",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE recipe_ingredient ri
                SET recipe_id = s.recipe_id
                FROM ingredient_section s
                WHERE s.ingredient_section_id = ri.ingredient_section_id;
                """);

            migrationBuilder.DropColumn(
                name: "ingredient_section_id",
                table: "recipe_ingredient");

            migrationBuilder.AlterColumn<int>(
                name: "recipe_id",
                table: "recipe_ingredient",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "ingredient_section");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ingredient__recipe_id",
                table: "recipe_ingredient",
                column: "recipe_id");

            migrationBuilder.AddForeignKey(
                name: "FK_recipe_ingredient__recipe_id",
                table: "recipe_ingredient",
                column: "recipe_id",
                principalTable: "recipe",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
