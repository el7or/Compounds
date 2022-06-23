using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class notification_units_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationUnits_CompoundNotifications_CompoundUnitId",
                table: "NotificationUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationUnits_CompoundUnits_CompoundNotificationId",
                table: "NotificationUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationUnits_CompoundNotifications_CompoundNotificationId",
                table: "NotificationUnits",
                column: "CompoundNotificationId",
                principalTable: "CompoundNotifications",
                principalColumn: "CompoundNotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationUnits_CompoundUnits_CompoundUnitId",
                table: "NotificationUnits",
                column: "CompoundUnitId",
                principalTable: "CompoundUnits",
                principalColumn: "CompoundUnitId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationUnits_CompoundNotifications_CompoundNotificationId",
                table: "NotificationUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationUnits_CompoundUnits_CompoundUnitId",
                table: "NotificationUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationUnits_CompoundNotifications_CompoundUnitId",
                table: "NotificationUnits",
                column: "CompoundUnitId",
                principalTable: "CompoundNotifications",
                principalColumn: "CompoundNotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationUnits_CompoundUnits_CompoundNotificationId",
                table: "NotificationUnits",
                column: "CompoundNotificationId",
                principalTable: "CompoundUnits",
                principalColumn: "CompoundUnitId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
