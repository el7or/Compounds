using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class notification_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "OwnerUnitId",
                table: "OwnerUnits");

            migrationBuilder.DropColumn(
                name: "NotifyDate",
                table: "CompoundNotifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "CompoundNotifications");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "CompoundNotifications",
                newName: "EnglishMessage");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "CompoundNotifications",
                type: "bit",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CompoundNotifications",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((1))");

            migrationBuilder.AddColumn<string>(
                name: "ArabicMessage",
                table: "CompoundNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArabicTitle",
                table: "CompoundNotifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "CompoundNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EnglishTitle",
                table: "CompoundNotifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "CompoundNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnerUnits",
                table: "OwnerUnits",
                column: "OwnerUnitId");

            migrationBuilder.CreateTable(
                name: "CompoundNotificationImages",
                columns: table => new
                {
                    CompoundNotificationImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNotificationImages", x => x.CompoundNotificationImageId);
                    table.ForeignKey(
                        name: "FK_CompoundNotificationImages_CompoundNotifications_CompoundNotificationId",
                        column: x => x.CompoundNotificationId,
                        principalTable: "CompoundNotifications",
                        principalColumn: "CompoundNotificationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationUnits",
                columns: table => new
                {
                    NotificationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUnits", x => x.NotificationUnitId);
                    table.ForeignKey(
                        name: "FK_NotificationUnits_CompoundNotifications_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundNotifications",
                        principalColumn: "CompoundNotificationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificationUnits_CompoundUnits_CompoundNotificationId",
                        column: x => x.CompoundNotificationId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerNotifications",
                columns: table => new
                {
                    OwnerNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerNotifications", x => x.OwnerNotificationId);
                    table.ForeignKey(
                        name: "FK_OwnerNotifications_CompoundNotifications_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "CompoundNotifications",
                        principalColumn: "CompoundNotificationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerNotifications_OwnerRegistrations_CompoundNotificationId",
                        column: x => x.CompoundNotificationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNotificationImages_CompoundNotificationId",
                table: "CompoundNotificationImages",
                column: "CompoundNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUnits_CompoundNotificationId",
                table: "NotificationUnits",
                column: "CompoundNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUnits_CompoundUnitId",
                table: "NotificationUnits",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerNotifications_CompoundNotificationId",
                table: "OwnerNotifications",
                column: "CompoundNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerNotifications_OwnerRegistrationId",
                table: "OwnerNotifications",
                column: "OwnerRegistrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompoundNotificationImages");

            migrationBuilder.DropTable(
                name: "NotificationUnits");

            migrationBuilder.DropTable(
                name: "OwnerNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnerUnits",
                table: "OwnerUnits");

            migrationBuilder.DropColumn(
                name: "ArabicMessage",
                table: "CompoundNotifications");

            migrationBuilder.DropColumn(
                name: "ArabicTitle",
                table: "CompoundNotifications");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "CompoundNotifications");

            migrationBuilder.DropColumn(
                name: "EnglishTitle",
                table: "CompoundNotifications");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "CompoundNotifications");

            migrationBuilder.RenameColumn(
                name: "EnglishMessage",
                table: "CompoundNotifications",
                newName: "Message");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "CompoundNotifications",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CompoundNotifications",
                type: "bit",
                nullable: true,
                defaultValueSql: "((1))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "((1))");

            migrationBuilder.AddColumn<DateTime>(
                name: "NotifyDate",
                table: "CompoundNotifications",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CompoundNotifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "OwnerUnitId",
                table: "OwnerUnits",
                column: "OwnerUnitId");
        }
    }
}
