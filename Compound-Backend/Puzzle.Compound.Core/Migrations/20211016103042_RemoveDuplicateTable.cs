using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class RemoveDuplicateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersInCompanyRoles");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "CompanyUserRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "CompanyUserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "CompanyUserRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "CompanyUserRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "CompanyUserRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "CompanyRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "CompanyRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "CompanyRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "CompanyRoles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "ActionsInCompanyRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreationUser",
                table: "ActionsInCompanyRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "ActionsInCompanyRoles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationUser",
                table: "ActionsInCompanyRoles",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "CompanyUserRoles");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "CompanyUserRoles");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "CompanyUserRoles");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "CompanyUserRoles");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ActionsInCompanyRoles");

            migrationBuilder.DropColumn(
                name: "CreationUser",
                table: "ActionsInCompanyRoles");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "ActionsInCompanyRoles");

            migrationBuilder.DropColumn(
                name: "ModificationUser",
                table: "ActionsInCompanyRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "CompanyUserRoles",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "UsersInCompanyRoles",
                columns: table => new
                {
                    UserCompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInCompanyRoles", x => x.UserCompanyRoleId);
                    table.ForeignKey(
                        name: "FK_UsersInCompanyRoles_CompanyRoles_CompanyRoleId",
                        column: x => x.CompanyRoleId,
                        principalTable: "CompanyRoles",
                        principalColumn: "CompanyRoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersInCompanyRoles_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersInCompanyRoles_CompanyRoleId",
                table: "UsersInCompanyRoles",
                column: "CompanyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInCompanyRoles_CompanyUserId",
                table: "UsersInCompanyRoles",
                column: "CompanyUserId");
        }
    }
}
