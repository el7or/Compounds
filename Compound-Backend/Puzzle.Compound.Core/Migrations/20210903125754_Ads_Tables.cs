using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class Ads_Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "CompoundAds");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CompoundAds",
                newName: "EnglishDescription");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CompoundAds",
                newName: "ArabicDescription");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "CompoundAds",
                type: "bit",
                nullable: false,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CompoundAds",
                type: "bit",
                nullable: false,
                defaultValueSql: "((1))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValueSql: "((1))");

            migrationBuilder.AddColumn<string>(
                name: "AdUrl",
                table: "CompoundAds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArabicTitle",
                table: "CompoundAds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "CompoundAds",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EnglishTitle",
                table: "CompoundAds",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "CompoundAds",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "CompoundAds",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CompoundAdHistories",
                columns: table => new
                {
                    CompoundAdHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundAdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    ClickedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundAdHistories", x => x.CompoundAdHistoryId);
                    table.ForeignKey(
                        name: "FK_CompoundAdHistories_CompoundAds_CompoundAdId",
                        column: x => x.CompoundAdId,
                        principalTable: "CompoundAds",
                        principalColumn: "CompoundAdId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundAdHistories_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundAdImages",
                columns: table => new
                {
                    CompoundAdImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundAdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundAdImages", x => x.CompoundAdImageId);
                    table.ForeignKey(
                        name: "FK_CompoundAdImages_CompoundAds_CompoundAdId",
                        column: x => x.CompoundAdId,
                        principalTable: "CompoundAds",
                        principalColumn: "CompoundAdId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompoundAdHistories_CompoundAdId",
                table: "CompoundAdHistories",
                column: "CompoundAdId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundAdHistories_OwnerRegistrationId",
                table: "CompoundAdHistories",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundAdImages_CompoundAdId",
                table: "CompoundAdImages",
                column: "CompoundAdId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompoundAdHistories");

            migrationBuilder.DropTable(
                name: "CompoundAdImages");

            migrationBuilder.DropColumn(
                name: "AdUrl",
                table: "CompoundAds");

            migrationBuilder.DropColumn(
                name: "ArabicTitle",
                table: "CompoundAds");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "CompoundAds");

            migrationBuilder.DropColumn(
                name: "EnglishTitle",
                table: "CompoundAds");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "CompoundAds");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "CompoundAds");

            migrationBuilder.RenameColumn(
                name: "EnglishDescription",
                table: "CompoundAds",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ArabicDescription",
                table: "CompoundAds",
                newName: "Description");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "CompoundAds",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "((0))");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CompoundAds",
                type: "bit",
                nullable: true,
                defaultValueSql: "((1))",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "((1))");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "CompoundAds",
                type: "bit",
                nullable: true,
                defaultValueSql: "((0))");
        }
    }
}
