using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationsAndRecipeMade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__cookbook_id",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__created_by",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__recipe_id",
                table: "cookbook_notification");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification__created_by",
                table: "cookbook_notification");

            migrationBuilder.AddColumn<int>(
                name: "made_count",
                table: "recipe",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "cookbook_id",
                table: "cookbook_notification",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "actor_user_id",
                table: "cookbook_notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "read_at",
                table: "cookbook_notification",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipient_user_id",
                table: "cookbook_notification",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "subject_user_id",
                table: "cookbook_notification",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recipe_made",
                columns: table => new
                {
                    recipe_made_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    made_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    made_on_utc_date = table.Column<DateOnly>(type: "date", nullable: false)
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
                name: "IX_cookbook_notification__recipient_read_created",
                table: "cookbook_notification",
                columns: new[] { "recipient_user_id", "read_at", "created" });

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipient_user_id",
                table: "cookbook_notification",
                column: "recipient_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification_actor_user_id",
                table: "cookbook_notification",
                column: "actor_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification_subject_user_id",
                table: "cookbook_notification",
                column: "subject_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_recipe_made__recipe_id",
                table: "recipe_made",
                column: "recipe_id");

            migrationBuilder.CreateIndex(
                name: "UX_recipe_made__user_recipe_day",
                table: "recipe_made",
                columns: new[] { "user_id", "recipe_id", "made_on_utc_date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__actor_user_id",
                table: "cookbook_notification",
                column: "actor_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__cookbook_id",
                table: "cookbook_notification",
                column: "cookbook_id",
                principalTable: "cookbook",
                principalColumn: "cookbook_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__recipe_id",
                table: "cookbook_notification",
                column: "recipe_id",
                principalTable: "recipe",
                principalColumn: "recipe_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__recipient_user_id",
                table: "cookbook_notification",
                column: "recipient_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__subject_user_id",
                table: "cookbook_notification",
                column: "subject_user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__actor_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__cookbook_id",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__recipe_id",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__recipient_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_notification__subject_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropTable(
                name: "recipe_made");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification__recipient_read_created",
                table: "cookbook_notification");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification__recipient_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification_actor_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification_subject_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "made_count",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "actor_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "read_at",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "recipient_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "subject_user_id",
                table: "cookbook_notification");

            migrationBuilder.AlterColumn<int>(
                name: "cookbook_id",
                table: "cookbook_notification",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__created_by",
                table: "cookbook_notification",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__cookbook_id",
                table: "cookbook_notification",
                column: "cookbook_id",
                principalTable: "cookbook",
                principalColumn: "cookbook_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__created_by",
                table: "cookbook_notification",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_notification__recipe_id",
                table: "cookbook_notification",
                column: "recipe_id",
                principalTable: "recipe",
                principalColumn: "recipe_id");
        }
    }
}
