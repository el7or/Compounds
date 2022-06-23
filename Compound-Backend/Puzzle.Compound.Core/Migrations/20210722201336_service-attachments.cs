using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class serviceattachments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Days",
                table: "ServiceRequests",
                newName: "Record");

            migrationBuilder.AlterColumn<DateTime>(
                name: "To",
                table: "ServiceRequests",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "From",
                table: "ServiceRequests",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ServiceAttachments",
                columns: table => new
                {
                    ServiceAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAttachments", x => x.ServiceAttachmentId);
                    table.ForeignKey(
                        name: "FK_ServiceAttachments_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "ServiceRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAttachments_ServiceRequestId",
                table: "ServiceAttachments",
                column: "ServiceRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAttachments");

            migrationBuilder.DropColumn(
                name: "date",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "Record",
                table: "ServiceRequests",
                newName: "Days");

            migrationBuilder.AlterColumn<DateTime>(
                name: "To",
                table: "ServiceRequests",
                type: "dateTime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "From",
                table: "ServiceRequests",
                type: "dateTime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
