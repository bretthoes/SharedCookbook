using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TightenIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_notification__recipient_user_id",
                table: "cookbook_notification");

            migrationBuilder.DropIndex(
                name: "IX_cookbook_member__cookbook_id",
                table: "cookbook_member");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token",
                column: "public_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_notification__recipient_user_id",
                table: "cookbook_notification",
                column: "recipient_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_cookbook_member__cookbook_id",
                table: "cookbook_member",
                column: "cookbook_id");
        }
    }
}
