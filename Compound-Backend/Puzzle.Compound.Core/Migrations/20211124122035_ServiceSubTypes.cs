using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class ServiceSubTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServiceSubTypesTotalCost",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ServiceSubTypes",
                columns: table => new
                {
                    ServiceSubTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSubTypes", x => x.ServiceSubTypeId);
                    table.ForeignKey(
                        name: "FK_ServiceSubTypes_CompoundServices_CompoundServiceId",
                        column: x => x.CompoundServiceId,
                        principalTable: "CompoundServices",
                        principalColumn: "CompoundServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequestSubTypes",
                columns: table => new
                {
                    ServiceRequestSubTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceSubTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceSubTypeCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ServiceSubTypeQuantity = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequestSubTypes", x => x.ServiceRequestSubTypeId);
                    table.ForeignKey(
                        name: "FK_ServiceRequestSubTypes_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "ServiceRequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequestSubTypes_ServiceSubTypes_ServiceSubTypeId",
                        column: x => x.ServiceSubTypeId,
                        principalTable: "ServiceSubTypes",
                        principalColumn: "ServiceSubTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestSubTypes_ServiceRequestId",
                table: "ServiceRequestSubTypes",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequestSubTypes_ServiceSubTypeId",
                table: "ServiceRequestSubTypes",
                column: "ServiceSubTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSubTypes_CompoundServiceId",
                table: "ServiceSubTypes",
                column: "CompoundServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceRequestSubTypes");

            migrationBuilder.DropTable(
                name: "ServiceSubTypes");

            migrationBuilder.DropColumn(
                name: "ServiceSubTypesTotalCost",
                table: "ServiceRequests");
        }
    }
}
