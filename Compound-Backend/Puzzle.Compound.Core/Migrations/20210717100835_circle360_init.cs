using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Puzzle.Compound.Core.Migrations
{
    public partial class circle360_init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyEmployees",
                columns: table => new
                {
                    CompanyEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEmployees", x => x.CompanyEmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRoles",
                columns: table => new
                {
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRoles", x => x.CompanyRoleId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundManagements",
                columns: table => new
                {
                    CompoundManagementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundManagements", x => x.CompoundManagementId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundResidents",
                columns: table => new
                {
                    CompoundResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundResidents", x => x.CompoundResidentId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundResidentsServices",
                columns: table => new
                {
                    CompoundResidentServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDatetime = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundResidentsServices", x => x.CompoundResidentServiceId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundSurveys",
                columns: table => new
                {
                    CompoundSurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundSurveys", x => x.CompoundSurveyId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundUnitTypes",
                columns: table => new
                {
                    CompoundUnitTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundUnitTypes", x => x.CompoundUnitTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundVisitors",
                columns: table => new
                {
                    CompoundVisitorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundVisitors", x => x.CompoundVisitorId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundVisitsRequests",
                columns: table => new
                {
                    CompoundVisitsRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundVisitsRequests", x => x.CompoundVisitsRequestId);
                });

            migrationBuilder.CreateTable(
                name: "Gates",
                columns: table => new
                {
                    GateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    GateName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false, comment: "1 = Entrance, 2 = Exit, 3 = All"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gates", x => x.GateId);
                });

            migrationBuilder.CreateTable(
                name: "OwnerRegistrations",
                columns: table => new
                {
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatsAppNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    UserType = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((1))", comment: "Owner => 1, Sub User => 2, Tenant => 3"),
                    MainRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerRegistrations", x => x.OwnerRegistrationId);
                });

            migrationBuilder.CreateTable(
                name: "PlanItems",
                columns: table => new
                {
                    PlanItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanItemNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PlanItemNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PlanItemDetailAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanItemDetailEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanItems", x => x.PlanItemId);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PlanNameAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlanNameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.PlanId);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshToken_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsExpired = table.Column<bool>(type: "bit", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User_Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshToken_Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    ServiceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ArabicName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsFixed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.ServiceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecordId = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrentJsonData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreviousJsonData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControllerPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActionPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                });

            migrationBuilder.CreateTable(
                name: "CompoundOwners",
                columns: table => new
                {
                    CompoundOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatsAppNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundOwners", x => x.CompoundOwnerId);
                    table.ForeignKey(
                        name: "FK_CompoundOwners_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name_Ar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name_En = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhatsAppNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompanyType = table.Column<int>(type: "int", nullable: true, comment: "Multi=>2, Single=>1"),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlanDetails",
                columns: table => new
                {
                    PlanDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanDetails", x => x.PlanDetailId);
                    table.ForeignKey(
                        name: "FK_PlanDetails_PlanItems_PlanItemId",
                        column: x => x.PlanItemId,
                        principalTable: "PlanItems",
                        principalColumn: "PlanItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlanDetails_Plans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    Online = table.Column<bool>(type: "bit", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "Gate ID, Employee ID, Company ID"),
                    UserType = table.Column<int>(type: "int", nullable: true, comment: "1 = Company, 2 = Employee, 3 = Gate")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => x.CompanyUserId);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compounds",
                columns: table => new
                {
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    TimeZoneText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TimeZoneOffset = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compounds", x => x.CompoundId);
                    table.ForeignKey(
                        name: "FK_Compounds_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyUserRoles",
                columns: table => new
                {
                    CompanyUserRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUserRoles", x => x.CompanyUserRoleId);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_CompanyRoles_CompanyRoleId",
                        column: x => x.CompanyRoleId,
                        principalTable: "CompanyRoles",
                        principalColumn: "CompanyRoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyUserRoles_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundAds",
                columns: table => new
                {
                    CompoundAdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundAds", x => x.CompoundAdId);
                    table.ForeignKey(
                        name: "FK_CompoundAds_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundAreas",
                columns: table => new
                {
                    CompoundAreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundAreas", x => x.CompoundAreaId);
                    table.ForeignKey(
                        name: "FK_CompoundAreas_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundCardPrintRequests",
                columns: table => new
                {
                    CompoundCardRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CardsCount = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundCardPrintRequests", x => x.CompoundCardRequestId);
                    table.ForeignKey(
                        name: "FK_CompoundCardPrintRequests_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundGates",
                columns: table => new
                {
                    CompoundGateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Gate_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundGates", x => x.CompoundGateId);
                    table.ForeignKey(
                        name: "FK_CompoundGates_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundGates_Gates_Gate_Id",
                        column: x => x.Gate_Id,
                        principalTable: "Gates",
                        principalColumn: "GateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundGroups",
                columns: table => new
                {
                    CompoundGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    NameAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    ParentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundGroups", x => x.CompoundGroupId);
                    table.ForeignKey(
                        name: "FK_CompoundGroups_CompoundGroups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "CompoundGroups",
                        principalColumn: "CompoundGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundGroups_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundHelps",
                columns: table => new
                {
                    CompoundHelpId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HelpContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundHelps", x => x.CompoundHelpId);
                    table.ForeignKey(
                        name: "FK_CompoundHelps_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundInstructions",
                columns: table => new
                {
                    CompoundInstructionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    InstructionContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundInstructions", x => x.CompoundInstructionId);
                    table.ForeignKey(
                        name: "FK_CompoundInstructions_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundNearbyPlaces",
                columns: table => new
                {
                    CompoundPlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    PlaceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNearbyPlaces", x => x.CompoundPlaceId);
                    table.ForeignKey(
                        name: "FK_CompoundNearbyPlaces_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundNews",
                columns: table => new
                {
                    CompoundNewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnglishTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArabicTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArabicSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArabicDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ForegroundTillDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNews", x => x.CompoundNewsId);
                    table.ForeignKey(
                        name: "FK_CompoundNews_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundNotices",
                columns: table => new
                {
                    CompoundNoticeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    NoticeContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoticeDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNotices", x => x.CompoundNoticeId);
                    table.ForeignKey(
                        name: "FK_CompoundNotices_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundNotices_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundNotifications",
                columns: table => new
                {
                    CompoundNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotifyDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNotifications", x => x.CompoundNotificationId);
                    table.ForeignKey(
                        name: "FK_CompoundNotifications_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundOwnerProperties",
                columns: table => new
                {
                    CompoundOwnerPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundOwnerProperties", x => x.CompoundOwnerPropertyId);
                    table.ForeignKey(
                        name: "FK_CompoundOwnerProperties_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundSecurities",
                columns: table => new
                {
                    CompoundSecurityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundSecurities", x => x.CompoundSecurityId);
                    table.ForeignKey(
                        name: "FK_CompoundSecurities_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundServices",
                columns: table => new
                {
                    CompoundServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ServiceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundServices", x => x.CompoundServiceId);
                    table.ForeignKey(
                        name: "FK_CompoundServices_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundServices_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundStores",
                columns: table => new
                {
                    CompoundStoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    StoreName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundStores", x => x.CompoundStoreId);
                    table.ForeignKey(
                        name: "FK_CompoundStores_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundUnits",
                columns: table => new
                {
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompoundGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundUnits", x => x.CompoundUnitId);
                    table.ForeignKey(
                        name: "FK_CompoundUnits_CompoundGroups_CompoundGroupId",
                        column: x => x.CompoundGroupId,
                        principalTable: "CompoundGroups",
                        principalColumn: "CompoundGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompoundUnits_CompoundUnitTypes_CompoundUnitTypeId",
                        column: x => x.CompoundUnitTypeId,
                        principalTable: "CompoundUnitTypes",
                        principalColumn: "CompoundUnitTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompoundNewsImages",
                columns: table => new
                {
                    CompoundNewsImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundNewsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompoundNewsImages", x => x.CompoundNewsImageId);
                    table.ForeignKey(
                        name: "FK_CompoundNewsImages_CompoundNews_CompoundNewsId",
                        column: x => x.CompoundNewsId,
                        principalTable: "CompoundNews",
                        principalColumn: "CompoundNewsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerAssignedUnits",
                columns: table => new
                {
                    OwnerAssignedUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    StartFrom = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTo = table.Column<DateTime>(type: "datetime", nullable: true),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerAssignedUnits", x => x.OwnerAssignedUnitId);
                    table.ForeignKey(
                        name: "FK_OwnerAssignedUnits_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerAssignedUnits_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerAssignUnitRequests",
                columns: table => new
                {
                    OwnerAssignUnitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Waiting = 0, Cancel = 1, Approved = 2"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerAssignUnitRequests", x => x.OwnerAssignUnitRequestId);
                    table.ForeignKey(
                        name: "FK_OwnerAssignUnitRequests_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerAssignUnitRequests_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OwnerUnits",
                columns: table => new
                {
                    OwnerUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("OwnerUnitId", x => x.OwnerUnitId);
                    table.ForeignKey(
                        name: "FK_OwnerUnits_CompoundOwners_CompoundOwnerId",
                        column: x => x.CompoundOwnerId,
                        principalTable: "CompoundOwners",
                        principalColumn: "CompoundOwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnerUnits_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    ServiceRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    ServiceTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    Days = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    From = table.Column<DateTime>(type: "dateTime", nullable: false),
                    To = table.Column<DateTime>(type: "dateTime", nullable: false),
                    CancelType = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.ServiceRequestId);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "ServiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitRequests",
                columns: table => new
                {
                    VisitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    VisitorName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CarNo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VisitType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GroupNo = table.Column<int>(type: "int", nullable: false),
                    Days = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    IsConsumed = table.Column<bool>(type: "bit", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    OwnerRegistrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitRequests", x => x.VisitRequestId);
                    table.ForeignKey(
                        name: "FK_VisitRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitRequests_Compounds_CompoundId",
                        column: x => x.CompoundId,
                        principalTable: "Compounds",
                        principalColumn: "CompoundId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitRequests_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitRequests_OwnerRegistrations_OwnerRegistrationId",
                        column: x => x.OwnerRegistrationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrintCardRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Picture = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    OwnerRegisterationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompoundUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintCardRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrintCardRequests_CompoundUnits_CompoundUnitId",
                        column: x => x.CompoundUnitId,
                        principalTable: "CompoundUnits",
                        principalColumn: "CompoundUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintCardRequests_OwnerRegistrations_OwnerRegisterationId",
                        column: x => x.OwnerRegisterationId,
                        principalTable: "OwnerRegistrations",
                        principalColumn: "OwnerRegistrationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrintCardRequests_VisitRequests_VisitRequestId",
                        column: x => x.VisitRequestId,
                        principalTable: "VisitRequests",
                        principalColumn: "VisitRequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VisitRequestAttachments",
                columns: table => new
                {
                    VisitRequestAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    Path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((1))"),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "((0))"),
                    VisitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitRequestAttachments", x => x.VisitRequestAttachmentId);
                    table.ForeignKey(
                        name: "FK_VisitRequestAttachments_VisitRequests_VisitRequestId",
                        column: x => x.VisitRequestId,
                        principalTable: "VisitRequests",
                        principalColumn: "VisitRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitTransactionHistory",
                columns: table => new
                {
                    VisitTransactionHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    VisitRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitTransactionHistory", x => x.VisitTransactionHistoryId);
                    table.ForeignKey(
                        name: "FK_VisitTransactionHistory_CompanyUsers_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUsers",
                        principalColumn: "CompanyUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VisitTransactionHistory_Gates_GateId",
                        column: x => x.GateId,
                        principalTable: "Gates",
                        principalColumn: "GateId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitTransactionHistory_VisitRequests_VisitRequestId",
                        column: x => x.VisitRequestId,
                        principalTable: "VisitRequests",
                        principalColumn: "VisitRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_PlanId",
                table: "Companies",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_CompanyRoleId",
                table: "CompanyUserRoles",
                column: "CompanyRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_CompanyUserId",
                table: "CompanyUserRoles",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUserRoles_CompoundId",
                table: "CompanyUserRoles",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_CompanyId",
                table: "CompanyUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundAds_CompoundId",
                table: "CompoundAds",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundAreas_CompoundId",
                table: "CompoundAreas",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundCardPrintRequests_CompoundId",
                table: "CompoundCardPrintRequests",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundGates_CompoundId",
                table: "CompoundGates",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundGates_Gate_Id",
                table: "CompoundGates",
                column: "Gate_Id");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundGroups_CompoundId",
                table: "CompoundGroups",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundGroups_ParentGroupId",
                table: "CompoundGroups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundHelps_CompoundId",
                table: "CompoundHelps",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundInstructions_CompoundId",
                table: "CompoundInstructions",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNearbyPlaces_CompoundId",
                table: "CompoundNearbyPlaces",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNews_CompoundId",
                table: "CompoundNews",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNewsImages_CompoundNewsId",
                table: "CompoundNewsImages",
                column: "CompoundNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNotices_CompanyUserId",
                table: "CompoundNotices",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNotices_CompoundId",
                table: "CompoundNotices",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundNotifications_CompoundId",
                table: "CompoundNotifications",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundOwnerProperties_CompoundId",
                table: "CompoundOwnerProperties",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundOwners_OwnerRegistrationId",
                table: "CompoundOwners",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_Compounds_CompanyId",
                table: "Compounds",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundSecurities_CompoundId",
                table: "CompoundSecurities",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundServices_CompoundId",
                table: "CompoundServices",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundServices_ServiceTypeId",
                table: "CompoundServices",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundStores_CompoundId",
                table: "CompoundStores",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundUnits_CompoundGroupId",
                table: "CompoundUnits",
                column: "CompoundGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CompoundUnits_CompoundUnitTypeId",
                table: "CompoundUnits",
                column: "CompoundUnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerAssignedUnits_CompoundUnitId",
                table: "OwnerAssignedUnits",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerAssignedUnits_OwnerRegistrationId",
                table: "OwnerAssignedUnits",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerAssignUnitRequests_CompoundUnitId",
                table: "OwnerAssignUnitRequests",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerAssignUnitRequests_OwnerRegistrationId",
                table: "OwnerAssignUnitRequests",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerUnits_CompoundOwnerId",
                table: "OwnerUnits",
                column: "CompoundOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerUnits_CompoundUnitId",
                table: "OwnerUnits",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanDetails_PlanId",
                table: "PlanDetails",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanDetails_PlanItemId",
                table: "PlanDetails",
                column: "PlanItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintCardRequests_CompoundUnitId",
                table: "PrintCardRequests",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintCardRequests_OwnerRegisterationId",
                table: "PrintCardRequests",
                column: "OwnerRegisterationId");

            migrationBuilder.CreateIndex(
                name: "IX_PrintCardRequests_VisitRequestId",
                table: "PrintCardRequests",
                column: "VisitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CompoundId",
                table: "ServiceRequests",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CompoundUnitId",
                table: "ServiceRequests",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_OwnerRegistrationId",
                table: "ServiceRequests",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceTypeId",
                table: "ServiceRequests",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequestAttachments_VisitRequestId",
                table: "VisitRequestAttachments",
                column: "VisitRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CompanyId",
                table: "VisitRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CompoundId",
                table: "VisitRequests",
                column: "CompoundId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_CompoundUnitId",
                table: "VisitRequests",
                column: "CompoundUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitRequests_OwnerRegistrationId",
                table: "VisitRequests",
                column: "OwnerRegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTransactionHistory_CompanyUserId",
                table: "VisitTransactionHistory",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTransactionHistory_GateId",
                table: "VisitTransactionHistory",
                column: "GateId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitTransactionHistory_VisitRequestId",
                table: "VisitTransactionHistory",
                column: "VisitRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyEmployees");

            migrationBuilder.DropTable(
                name: "CompanyUserRoles");

            migrationBuilder.DropTable(
                name: "CompoundAds");

            migrationBuilder.DropTable(
                name: "CompoundAreas");

            migrationBuilder.DropTable(
                name: "CompoundCardPrintRequests");

            migrationBuilder.DropTable(
                name: "CompoundGates");

            migrationBuilder.DropTable(
                name: "CompoundHelps");

            migrationBuilder.DropTable(
                name: "CompoundInstructions");

            migrationBuilder.DropTable(
                name: "CompoundManagements");

            migrationBuilder.DropTable(
                name: "CompoundNearbyPlaces");

            migrationBuilder.DropTable(
                name: "CompoundNewsImages");

            migrationBuilder.DropTable(
                name: "CompoundNotices");

            migrationBuilder.DropTable(
                name: "CompoundNotifications");

            migrationBuilder.DropTable(
                name: "CompoundOwnerProperties");

            migrationBuilder.DropTable(
                name: "CompoundResidents");

            migrationBuilder.DropTable(
                name: "CompoundResidentsServices");

            migrationBuilder.DropTable(
                name: "CompoundSecurities");

            migrationBuilder.DropTable(
                name: "CompoundServices");

            migrationBuilder.DropTable(
                name: "CompoundStores");

            migrationBuilder.DropTable(
                name: "CompoundSurveys");

            migrationBuilder.DropTable(
                name: "CompoundVisitors");

            migrationBuilder.DropTable(
                name: "CompoundVisitsRequests");

            migrationBuilder.DropTable(
                name: "OwnerAssignedUnits");

            migrationBuilder.DropTable(
                name: "OwnerAssignUnitRequests");

            migrationBuilder.DropTable(
                name: "OwnerUnits");

            migrationBuilder.DropTable(
                name: "PlanDetails");

            migrationBuilder.DropTable(
                name: "PrintCardRequests");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "VisitRequestAttachments");

            migrationBuilder.DropTable(
                name: "VisitTransactionHistory");

            migrationBuilder.DropTable(
                name: "CompanyRoles");

            migrationBuilder.DropTable(
                name: "CompoundNews");

            migrationBuilder.DropTable(
                name: "CompoundOwners");

            migrationBuilder.DropTable(
                name: "PlanItems");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "Gates");

            migrationBuilder.DropTable(
                name: "VisitRequests");

            migrationBuilder.DropTable(
                name: "CompoundUnits");

            migrationBuilder.DropTable(
                name: "OwnerRegistrations");

            migrationBuilder.DropTable(
                name: "CompoundGroups");

            migrationBuilder.DropTable(
                name: "CompoundUnitTypes");

            migrationBuilder.DropTable(
                name: "Compounds");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Plans");
        }
    }
}
