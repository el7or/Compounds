using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class IssuesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompoundResidentsIssues",
                columns: table => new
                {
                    CompoundResidentIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundResidentsIssues", x => x.CompoundResidentIssueId);
                });

            migrationBuilder.CreateTable(
                name: "IssueTypes",
                columns: table => new
                {
                    IssueTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ArabicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueTypes", x => x.IssueTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserIssues",
                columns: table => new
                {
                    CompanyUserIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompanyUserCompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserIssues", x => x.CompanyUserIssueId);
                    table.ForeignKey(
                        name: "FK_CompanyUserIssues_CompanyUserCompounds_CompanyUserCompoundId",
                        column: x => x.CompanyUserCompoundId,
                        principalTable: "CompanyUserCompounds",
                        principalColumn: "CompanyUserCompoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserIssues_IssueTypes_IssueTypeId",
                        column: x => x.IssueTypeId,
                        principalTable: "IssueTypes",
                        principalColumn: "IssueTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundIssues",
                columns: table => new
                {
                    CompoundIssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IssueTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundIssues", x => x.CompoundIssueId);
                    table.ForeignKey(
                        name: "FK_CompoundIssues_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundIssues_IssueTypes_IssueTypeId",
                        column: x => x.IssueTypeId,
                        principalTable: "IssueTypes",
                        principalColumn: "IssueTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueRequests",
                columns: table => new
                {
                    IssueRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IssueTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestNumber = table.Column<int>(type: "int", nullable: false),
                    PostTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    UpdateStatusTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdateStatusBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Rate = table.Column<short>(type: "smallint", nullable: false),
                    PresenterRate = table.Column<short>(type: "smallint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OwnerComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    CancelType = table.Column<short>(type: "smallint", nullable: false),
                    Record = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueRequests", x => x.IssueRequestId);
                    table.ForeignKey(
                        name: "FK_IssueRequests_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IssueRequests_IssueTypes_IssueTypeId",
                        column: x => x.IssueTypeId,
                        principalTable: "IssueTypes",
                        principalColumn: "IssueTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IssueRequests_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IssueAttachments",
                columns: table => new
                {
                    IssueAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IssueRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueAttachments", x => x.IssueAttachmentId);
                    table.ForeignKey(
                        name: "FK_IssueAttachments_IssueRequests_IssueRequestId",
                        column: x => x.IssueRequestId,
                        principalTable: "IssueRequests",
                        principalColumn: "IssueRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserIssues_CompanyUserCompoundId",
                table: "CompanyUserIssues",
                column: "CompanyUserCompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserIssues_IssueTypeId",
                table: "CompanyUserIssues",
                column: "IssueTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundIssues_CompoundId",
                table: "CompoundIssues",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundIssues_IssueTypeId",
                table: "CompoundIssues",
                column: "IssueTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueAttachments_IssueRequestId",
                table: "IssueAttachments",
                column: "IssueRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRequests_CompoundId",
                table: "IssueRequests",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRequests_IssueTypeId",
                table: "IssueRequests",
                column: "IssueTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueRequests_OwnerRegistrationId",
                table: "IssueRequests",
                column: "OwnerRegistrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "CompanyUserIssues");

            migrationBuilder.DropTable(
                name: "CompoundIssues");

            migrationBuilder.DropTable(
                name: "CompoundResidentsIssues");

            migrationBuilder.DropTable(
                name: "IssueAttachments");

            migrationBuilder.DropTable(
                name: "IssueRequests");

            migrationBuilder.DropTable(
                name: "IssueTypes");
        }
    }
}
