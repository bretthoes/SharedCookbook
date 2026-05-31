using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharedCookbook.Infrastructure.Data.Migrations;

/// <inheritdoc />
public partial class RemoveNotificationReadState : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_cookbook_notification__recipient_read_created",
            table: "cookbook_notification");

        migrationBuilder.DropColumn(
            name: "read_at",
            table: "cookbook_notification");

        migrationBuilder.CreateIndex(
            name: "IX_cookbook_notification__recipient_created",
            table: "cookbook_notification",
            columns: new[] { "recipient_user_id", "created" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_cookbook_notification__recipient_created",
            table: "cookbook_notification");

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "read_at",
            table: "cookbook_notification",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_cookbook_notification__recipient_read_created",
            table: "cookbook_notification",
            columns: new[] { "recipient_user_id", "read_at", "created" });
    }
}
