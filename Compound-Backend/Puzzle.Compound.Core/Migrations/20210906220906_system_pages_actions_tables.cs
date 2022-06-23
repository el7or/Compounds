using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class system_pages_actions_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "CompanyRoles");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "CompanyRoles",
                newName: "RoleEnglishName");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "CompanyRoles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "RoleArabicName",
                table: "CompanyRoles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SystemPage",
                columns: table => new
                {
                    SystemPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PageArabicName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PageEnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageIndex = table.Column<int>(type: "int", nullable: false),
                    PageURL = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPage", x => x.SystemPageId);
                    table.ForeignKey(
                        name: "FK_SystemPage_SystemPage_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "SystemPage",
                        principalColumn: "SystemPageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersInCompanyRoles",
                columns: table => new
                {
                    UserCompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInCompanyRoles_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemPageAction",
                columns: table => new
                {
                    SystemPageActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ActionArabicName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ActionEnglishName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ActionUniqueName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    SystemPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPageAction", x => x.SystemPageActionId);
                    table.ForeignKey(
                        name: "FK_SystemPageAction_SystemPage_SystemPageId",
                        column: x => x.SystemPageId,
                        principalTable: "SystemPage",
                        principalColumn: "SystemPageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionsInCompanyRoles",
                columns: table => new
                {
                    ActionsInCompanyRolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompanyUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemPageActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionsInCompanyRoles", x => x.ActionsInCompanyRolesId);
                    table.ForeignKey(
                        name: "FK_ActionsInCompanyRoles_CompanyUserRoles_CompanyUserRoleId",
                        column: x => x.CompanyUserRoleId,
                        principalTable: "CompanyUserRoles",
                        principalColumn: "CompanyUserRoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActionsInCompanyRoles_SystemPageAction_SystemPageActionId",
                        column: x => x.SystemPageActionId,
                        principalTable: "SystemPageAction",
                        principalColumn: "SystemPageActionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionsInCompanyRoles_CompanyUserRoleId",
                table: "ActionsInCompanyRoles",
                column: "CompanyUserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionsInCompanyRoles_SystemPageActionId",
                table: "ActionsInCompanyRoles",
                column: "SystemPageActionId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPage_ParentPageId",
                table: "SystemPage",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemPageAction_ActionUniqueName",
                table: "SystemPageAction",
                column: "ActionUniqueName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemPageAction_SystemPageId",
                table: "SystemPageAction",
                column: "SystemPageId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInCompanyRoles_CompanyRoleId",
                table: "UsersInCompanyRoles",
                column: "CompanyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInCompanyRoles_CompanyUserId",
                table: "UsersInCompanyRoles",
                column: "CompanyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyRoles_Companies_CompanyId",
                table: "CompanyRoles",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyRoles_Companies_CompanyId",
                table: "CompanyRoles");

            migrationBuilder.DropTable(
                name: "ActionsInCompanyRoles");

            migrationBuilder.DropTable(
                name: "UsersInCompanyRoles");

            migrationBuilder.DropTable(
                name: "SystemPageAction");

            migrationBuilder.DropTable(
                name: "SystemPage");

            migrationBuilder.DropIndex(
                name: "IX_CompanyRoles_CompanyId",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanyRoles");

            migrationBuilder.DropColumn(
                name: "RoleArabicName",
                table: "CompanyRoles");

            migrationBuilder.RenameColumn(
                name: "RoleEnglishName",
                table: "CompanyRoles",
                newName: "RoleName");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CompanyRoles",
                type: "bit",
                nullable: true,
                defaultValueSql: "((1))");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CompanyRoles",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "CompanyRoles",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    GroupArabicName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GroupEnglishName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                });

            migrationBuilder.CreateTable(
                name: "SystemPages",
                columns: table => new
                {
                    SystemPagesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ControlName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PageArabicName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PageEnglishName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PageIndex = table.Column<int>(type: "int", nullable: false),
                    PageURL = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentPageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemPages", x => x.SystemPagesId);
                    table.ForeignKey(
                        name: "FK_SystemPages_SystemPages_ParentPageId",
                        column: x => x.ParentPageId,
                        principalTable: "SystemPages",
                        principalColumn: "SystemPagesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersInGroup",
                columns: table => new
                {
                    UserGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersInGroup", x => x.UserGroupId);
                    table.ForeignKey(
                        name: "FK_UsersInGroup_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersInGroup_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemPages_ParentPageId",
                table: "SystemPages",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInGroup_CompanyUserId",
                table: "UsersInGroup",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersInGroup_GroupId",
                table: "UsersInGroup",
                column: "GroupId");
        }
    }
}
