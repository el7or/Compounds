using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class compoundtimezonevalue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneValue",
                table: "Compounds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneValue",
                table: "Compounds");
        }
    }
}
