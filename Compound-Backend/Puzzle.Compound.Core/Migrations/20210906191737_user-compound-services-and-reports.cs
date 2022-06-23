using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class usercompoundservicesandreports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUserReports_CompanyUsers_CompanyUserId",
                table: "CompanyUserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUserServices_CompanyUsers_CompanyUserId",
                table: "CompanyUserServices");

            migrationBuilder.RenameColumn(
                name: "CompanyUserId",
                table: "CompanyUserServices",
                newName: "CompanyUserCompoundId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUserServices_CompanyUserId",
                table: "CompanyUserServices",
                newName: "IX_CompanyUserServices_CompanyUserCompoundId");

            migrationBuilder.RenameColumn(
                name: "CompanyUserId",
                table: "CompanyUserReports",
                newName: "CompanyUserCompoundId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUserReports_CompanyUserId",
                table: "CompanyUserReports",
                newName: "IX_CompanyUserReports_CompanyUserCompoundId");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "CompanyUsers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUserReports_CompanyUserCompounds_CompanyUserCompoundId",
                table: "CompanyUserReports",
                column: "CompanyUserCompoundId",
                principalTable: "CompanyUserCompounds",
                principalColumn: "CompanyUserCompoundId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUserServices_CompanyUserCompounds_CompanyUserCompoundId",
                table: "CompanyUserServices",
                column: "CompanyUserCompoundId",
                principalTable: "CompanyUserCompounds",
                principalColumn: "CompanyUserCompoundId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUserReports_CompanyUserCompounds_CompanyUserCompoundId",
                table: "CompanyUserReports");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyUserServices_CompanyUserCompounds_CompanyUserCompoundId",
                table: "CompanyUserServices");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "CompanyUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyUserCompoundId",
                table: "CompanyUserServices",
                newName: "CompanyUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUserServices_CompanyUserCompoundId",
                table: "CompanyUserServices",
                newName: "IX_CompanyUserServices_CompanyUserId");

            migrationBuilder.RenameColumn(
                name: "CompanyUserCompoundId",
                table: "CompanyUserReports",
                newName: "CompanyUserId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanyUserReports_CompanyUserCompoundId",
                table: "CompanyUserReports",
                newName: "IX_CompanyUserReports_CompanyUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUserReports_CompanyUsers_CompanyUserId",
                table: "CompanyUserReports",
                column: "CompanyUserId",
                principalTable: "CompanyUsers",
                principalColumn: "CompanyUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyUserServices_CompanyUsers_CompanyUserId",
                table: "CompanyUserServices",
                column: "CompanyUserId",
                principalTable: "CompanyUsers",
                principalColumn: "CompanyUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
