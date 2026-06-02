using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayNamesToEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorDisplayName",
                table: "recipe",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorDisplayName",
                table: "cookbook_notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubjectDisplayName",
                table: "cookbook_notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "cookbook_member",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderDisplayName",
                table: "cookbook_invitation",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorDisplayName",
                table: "recipe");

            migrationBuilder.DropColumn(
                name: "ActorDisplayName",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "SubjectDisplayName",
                table: "cookbook_notification");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "cookbook_member");

            migrationBuilder.DropColumn(
                name: "SenderDisplayName",
                table: "cookbook_invitation");
        }
    }
}
