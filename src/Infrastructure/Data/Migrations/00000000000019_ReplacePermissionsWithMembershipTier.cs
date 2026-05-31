using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class ReplacePermissionsWithMembershipTier : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "tier",
            table: "cookbook_member",
            type: "integer",
            nullable: false,
            defaultValue: 1);

        // Map legacy bool columns to MembershipTier:
        // Owner = 3, Admin = 2, Contributor = 1, Viewer = 0
        migrationBuilder.Sql("""
            UPDATE cookbook_member
            SET tier = CASE
                WHEN is_owner THEN 3
                WHEN can_remove_member THEN 2
                WHEN can_add_recipe THEN 1
                ELSE 0
            END
            """);

        migrationBuilder.DropColumn(name: "can_add_recipe", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "can_delete_recipe", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "can_edit_cookbook_details", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "can_remove_member", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "can_send_invite", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "can_update_recipe", table: "cookbook_member");
        migrationBuilder.DropColumn(name: "is_owner", table: "cookbook_member");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "tier", table: "cookbook_member");

        migrationBuilder.AddColumn<bool>(name: "can_add_recipe", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "can_delete_recipe", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "can_edit_cookbook_details", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "can_remove_member", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "can_send_invite", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "can_update_recipe", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "is_owner", table: "cookbook_member", type: "boolean", nullable: false, defaultValue: false);
    }
}
