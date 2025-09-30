using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateInvitationLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           // migrationBuilder.DropColumn(
           //     name: "hash",
           //     table: "cookbook_invitation");

           // migrationBuilder.DropColumn(
           //     name: "salt",
           //     table: "cookbook_invitation");

            migrationBuilder.CreateTable(
                name: "invitation_token",
                columns: table => new
                {
                    invitation_token_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cookbook_invitation_id = table.Column<int>(type: "integer", nullable: false),
                    token_status = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    token_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    token_salt = table.Column<byte[]>(type: "bytea", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("invitation_token_id", x => x.invitation_token_id);
                    table.ForeignKey(
                        name: "FK_cookbook_invitation__invitation_token",
                        column: x => x.cookbook_invitation_id,
                        principalTable: "cookbook_invitation",
                        principalColumn: "cookbook_invitation_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token__invitation_status",
                table: "invitation_token",
                columns: new[] { "cookbook_invitation_id", "token_status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invitation_token");

             //migrationBuilder.AddColumn<byte[]>(
             //   name: "hash",
             //   table: "cookbook_invitation",
             //   type: "bytea",
             //   maxLength: 32,
             //   nullable: false,
             //   defaultValue: new byte[0]);

            //igrationBuilder.AddColumn<byte[]>(
             //   name: "salt",
             //   table: "cookbook_invitation",
             //   type: "bytea",
             //   maxLength: 16,
             //   nullable: false,
             //   defaultValue: new byte[0]);
        }
    }
}
