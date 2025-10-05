using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_creator",
                table: "cookbook_member",
                newName: "is_owner");

            migrationBuilder.CreateIndex(
                name: "UX_cookbook_member__cookbook_user",
                table: "cookbook_member",
                columns: new[] { "cookbook_id", "CreatedBy" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_cookbook_member__cookbook_user",
                table: "cookbook_member");

            migrationBuilder.RenameColumn(
                name: "is_owner",
                table: "cookbook_member",
                newName: "is_creator");
        }
    }
}
