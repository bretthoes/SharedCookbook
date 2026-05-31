using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class DropCookbookOwnerUniqueIndex : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "UX_cookbook_member__cookbook_owner",
            table: "cookbook_member");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE UNIQUE INDEX IF NOT EXISTS "UX_cookbook_member__cookbook_owner"
            ON cookbook_member (cookbook_id)
            WHERE tier = 3;
            """);
    }
}
