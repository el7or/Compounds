using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class gateuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyUserId",
                table: "Gates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gates_CompanyUserId",
                table: "Gates",
                column: "CompanyUserId",
                unique: true,
                filter: "[CompanyUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Gates_CompanyUsers_CompanyUserId",
                table: "Gates",
                column: "CompanyUserId",
                principalTable: "CompanyUsers",
                principalColumn: "CompanyUserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gates_CompanyUsers_CompanyUserId",
                table: "Gates");

            migrationBuilder.DropIndex(
                name: "IX_Gates_CompanyUserId",
                table: "Gates");

            migrationBuilder.DropColumn(
                name: "CompanyUserId",
                table: "Gates");
        }
    }
}
