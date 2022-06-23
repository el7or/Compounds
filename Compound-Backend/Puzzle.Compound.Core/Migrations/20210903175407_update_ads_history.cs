using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class update_ads_history : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickedOn",
                table: "CompoundAdHistories");

            migrationBuilder.RenameColumn(
                name: "ShowedOn",
                table: "CompoundAdHistories",
                newName: "ActionDate");

            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "CompoundAdHistories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Show = 1, Click = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "CompoundAdHistories");

            migrationBuilder.RenameColumn(
                name: "ActionDate",
                table: "CompoundAdHistories",
                newName: "ShowedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClickedOn",
                table: "CompoundAdHistories",
                type: "datetime",
                nullable: true);
        }
    }
}
