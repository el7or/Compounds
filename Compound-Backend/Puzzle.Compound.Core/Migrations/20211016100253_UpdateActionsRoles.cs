using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class UpdateActionsRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionsInCompanyRoles_CompanyUserRoles_CompanyUserRoleId",
                table: "ActionsInCompanyRoles");

            migrationBuilder.RenameColumn(
                name: "CompanyUserRoleId",
                table: "ActionsInCompanyRoles",
                newName: "CompanyRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ActionsInCompanyRoles_CompanyUserRoleId",
                table: "ActionsInCompanyRoles",
                newName: "IX_ActionsInCompanyRoles_CompanyRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionsInCompanyRoles_CompanyRoles_CompanyRoleId",
                table: "ActionsInCompanyRoles",
                column: "CompanyRoleId",
                principalTable: "CompanyRoles",
                principalColumn: "CompanyRoleId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActionsInCompanyRoles_CompanyRoles_CompanyRoleId",
                table: "ActionsInCompanyRoles");

            migrationBuilder.RenameColumn(
                name: "CompanyRoleId",
                table: "ActionsInCompanyRoles",
                newName: "CompanyUserRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ActionsInCompanyRoles_CompanyRoleId",
                table: "ActionsInCompanyRoles",
                newName: "IX_ActionsInCompanyRoles_CompanyUserRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActionsInCompanyRoles_CompanyUserRoles_CompanyUserRoleId",
                table: "ActionsInCompanyRoles",
                column: "CompanyUserRoleId",
                principalTable: "CompanyUserRoles",
                principalColumn: "CompanyUserRoleId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
