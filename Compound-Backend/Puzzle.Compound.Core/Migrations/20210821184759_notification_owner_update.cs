using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class notification_owner_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerNotifications_CompoundNotifications_OwnerRegistrationId",
                table: "OwnerNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnerNotifications_OwnerRegistrations_CompoundNotificationId",
                table: "OwnerNotifications");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerNotifications_CompoundNotifications_CompoundNotificationId",
                table: "OwnerNotifications",
                column: "CompoundNotificationId",
                principalTable: "CompoundNotifications",
                principalColumn: "CompoundNotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerNotifications_OwnerRegistrations_OwnerRegistrationId",
                table: "OwnerNotifications",
                column: "OwnerRegistrationId",
                principalTable: "OwnerRegistrations",
                principalColumn: "OwnerRegistrationId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerNotifications_CompoundNotifications_CompoundNotificationId",
                table: "OwnerNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnerNotifications_OwnerRegistrations_OwnerRegistrationId",
                table: "OwnerNotifications");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerNotifications_CompoundNotifications_OwnerRegistrationId",
                table: "OwnerNotifications",
                column: "OwnerRegistrationId",
                principalTable: "CompoundNotifications",
                principalColumn: "CompoundNotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerNotifications_OwnerRegistrations_CompoundNotificationId",
                table: "OwnerNotifications",
                column: "CompoundNotificationId",
                principalTable: "OwnerRegistrations",
                principalColumn: "OwnerRegistrationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
