using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class company_charge_name : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChargeName",
                table: "Companies",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChargeName",
                table: "Companies");
        }
    }
}
