using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHashAndSaltToInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          //  migrationBuilder.AddColumn<byte[]>(
          //      name: "hash",
          //      table: "cookbook_invitation",
          //      type: "bytea",
          //      maxLength: 32,
          //      nullable: false,
          //      defaultValue: new byte[0]);

          //  migrationBuilder.AddColumn<byte[]>(
          //      name: "salt",
          //      table: "cookbook_invitation",
          //      type: "bytea",
          //      maxLength: 16,
          //      nullable: false,
          //      defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          //  migrationBuilder.DropColumn(
          //      name: "hash",
          //      table: "cookbook_invitation");

          //  migrationBuilder.DropColumn(
          //      name: "salt",
          //      table: "cookbook_invitation");
        }
    }
}
