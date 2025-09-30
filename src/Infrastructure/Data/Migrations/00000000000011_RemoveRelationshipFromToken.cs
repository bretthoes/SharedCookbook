using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRelationshipFromToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cookbook_invitation__invitation_token",
                table: "invitation_token");

            migrationBuilder.RenameColumn(
                name: "token_status",
                table: "invitation_token",
                newName: "invitation_status");

            migrationBuilder.RenameColumn(
                name: "cookbook_invitation_id",
                table: "invitation_token",
                newName: "cookbook_id");

            migrationBuilder.RenameIndex(
                name: "IX_invitation_token_cookbook_invitation_id",
                table: "invitation_token",
                newName: "IX_invitation_token_cookbook_id");

            migrationBuilder.AddColumn<string>(
                name: "redeemer_person_id",
                table: "invitation_token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "response_date",
                table: "invitation_token",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_invitation_token_CreatedBy",
                table: "invitation_token",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_token__cookbook_id",
                table: "invitation_token",
                column: "cookbook_id",
                principalTable: "cookbook",
                principalColumn: "cookbook_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_invitation_token__created_by",
                table: "invitation_token",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invitation_token__cookbook_id",
                table: "invitation_token");

            migrationBuilder.DropForeignKey(
                name: "FK_invitation_token__created_by",
                table: "invitation_token");

            migrationBuilder.DropIndex(
                name: "IX_invitation_token_CreatedBy",
                table: "invitation_token");

            migrationBuilder.DropColumn(
                name: "redeemer_person_id",
                table: "invitation_token");

            migrationBuilder.DropColumn(
                name: "response_date",
                table: "invitation_token");

            migrationBuilder.RenameColumn(
                name: "invitation_status",
                table: "invitation_token",
                newName: "token_status");

            migrationBuilder.RenameColumn(
                name: "cookbook_id",
                table: "invitation_token",
                newName: "cookbook_invitation_id");

            migrationBuilder.RenameIndex(
                name: "IX_invitation_token_cookbook_id",
                table: "invitation_token",
                newName: "IX_invitation_token_cookbook_invitation_id");

            migrationBuilder.AddForeignKey(
                name: "FK_cookbook_invitation__invitation_token",
                table: "invitation_token",
                column: "cookbook_invitation_id",
                principalTable: "cookbook_invitation",
                principalColumn: "cookbook_invitation_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
