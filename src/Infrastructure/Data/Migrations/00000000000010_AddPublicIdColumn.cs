using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invitation_token__invitation_status",
                table: "invitation_token");

            migrationBuilder.AddColumn<Guid>(
                name: "public_id",
                table: "invitation_token",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token",
                column: "public_id");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token_cookbook_invitation_id",
                table: "invitation_token",
                column: "cookbook_invitation_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_invitation_token__public_id",
                table: "invitation_token");

            migrationBuilder.DropIndex(
                name: "IX_invitation_token_cookbook_invitation_id",
                table: "invitation_token");

            migrationBuilder.DropColumn(
                name: "public_id",
                table: "invitation_token");

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__invitation_status",
                table: "invitation_token",
                columns: new[] { "cookbook_invitation_id", "token_status" });
        }
    }
}
