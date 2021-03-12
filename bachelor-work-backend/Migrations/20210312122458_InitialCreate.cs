using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace bachelor_work_backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    katedra = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fakulta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    surname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    confirmed = table.Column<bool>(type: "bit", nullable: false),
                    guid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectInYear",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    year = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    subjectId = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectInYear", x => x.id);
                    table.ForeignKey(
                        name: "FK_SubjectInYear_Subject",
                        column: x => x.subjectId,
                        principalTable: "Subject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordRecovery",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    validUntil = table.Column<DateTime>(type: "datetime", nullable: false),
                    guid = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordRecovery", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserPasswordRecovery_User",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectInYearTerm",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectInYearId = table.Column<int>(type: "int", nullable: false),
                    term = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectInYearTerm", x => x.id);
                    table.ForeignKey(
                        name: "FK_SubjectInYearTerm_SubjectInYear",
                        column: x => x.subjectInYearId,
                        principalTable: "SubjectInYear",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Block",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectInYearTermId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Block", x => x.id);
                    table.ForeignKey(
                        name: "FK_Block_SubjectInYearTerm",
                        column: x => x.subjectInYearTermId,
                        principalTable: "SubjectInYearTerm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TermStagConnection",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subjectInYearTermId = table.Column<int>(type: "int", nullable: false),
                    zkrPredm = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    year = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    term = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermStagConnection", x => x.id);
                    table.ForeignKey(
                        name: "FK_TermStagConnection_SubjectInYearTerm",
                        column: x => x.subjectInYearTermId,
                        principalTable: "SubjectInYearTerm",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockAction",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    attendanceAllowStartDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    attendanceAllowEndDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    attendanceSignOffEndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    visible = table.Column<bool>(type: "bit", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    groupId = table.Column<int>(type: "int", nullable: false),
                    ucitIdno = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockAction", x => x.id);
                    table.ForeignKey(
                        name: "FK_BlockAction_Block",
                        column: x => x.blockId,
                        principalTable: "Block",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockRestriction",
                columns: table => new
                {
                    blockId = table.Column<int>(type: "int", nullable: false),
                    allowOnlyStudentsOnWhiteList = table.Column<bool>(type: "bit", nullable: false),
                    allowExternalUsers = table.Column<bool>(type: "bit", nullable: false),
                    actionAttendLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockRestriction", x => x.blockId);
                    table.ForeignKey(
                        name: "FK_BlockRestriction_Block",
                        column: x => x.blockId,
                        principalTable: "Block",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockStagUserWhitelist",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blockId = table.Column<int>(type: "int", nullable: false),
                    studentOsCislo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockStagUserWhitelist", x => x.id);
                    table.ForeignKey(
                        name: "FK_BlockStagUserWhitelist_Block",
                        column: x => x.blockId,
                        principalTable: "Block",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockActionAttendance",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    actionId = table.Column<int>(type: "int", nullable: false),
                    studentOsCislo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true),
                    fulfilled = table.Column<bool>(type: "bit", nullable: false),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false),
                    evaluationDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockActionAttendance", x => x.id);
                    table.ForeignKey(
                        name: "FK_BlockActionAttendance_BlockAction",
                        column: x => x.actionId,
                        principalTable: "BlockAction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlockActionAttendance_User",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockActionPeopleEnrollQueue",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    actionId = table.Column<int>(type: "int", nullable: false),
                    studentOsCislo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    userId = table.Column<int>(type: "int", nullable: true),
                    dateIn = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockActionPeopleEnrollQueue", x => x.id);
                    table.ForeignKey(
                        name: "FK_BlockActionPeopleEnrollQueue_BlockAction",
                        column: x => x.actionId,
                        principalTable: "BlockAction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BlockActionPeopleEnrollQueue_User",
                        column: x => x.userId,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlockActionRestriction",
                columns: table => new
                {
                    actionId = table.Column<int>(type: "int", nullable: false),
                    allowOnlyStudentsOnWhiteList = table.Column<bool>(type: "bit", nullable: false),
                    allowExternalUsers = table.Column<bool>(type: "bit", nullable: false),
                    maxCapacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockActionRestriction", x => x.actionId);
                    table.ForeignKey(
                        name: "FK_BlockActionRestriction_BlockAction",
                        column: x => x.actionId,
                        principalTable: "BlockAction",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Block_subjectInYearTermId",
                table: "Block",
                column: "subjectInYearTermId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockAction_blockId",
                table: "BlockAction",
                column: "blockId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockActionAttendance_actionId",
                table: "BlockActionAttendance",
                column: "actionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockActionAttendance_userId",
                table: "BlockActionAttendance",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockActionPeopleEnrollQueue_actionId",
                table: "BlockActionPeopleEnrollQueue",
                column: "actionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockActionPeopleEnrollQueue_userId",
                table: "BlockActionPeopleEnrollQueue",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockStagUserWhitelist_blockId",
                table: "BlockStagUserWhitelist",
                column: "blockId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInYear_subjectId",
                table: "SubjectInYear",
                column: "subjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectInYearTerm_subjectInYearId",
                table: "SubjectInYearTerm",
                column: "subjectInYearId");

            migrationBuilder.CreateIndex(
                name: "IX_TermStagConnection_subjectInYearTermId",
                table: "TermStagConnection",
                column: "subjectInYearTermId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordRecovery_userId",
                table: "UserPasswordRecovery",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockActionAttendance");

            migrationBuilder.DropTable(
                name: "BlockActionPeopleEnrollQueue");

            migrationBuilder.DropTable(
                name: "BlockActionRestriction");

            migrationBuilder.DropTable(
                name: "BlockRestriction");

            migrationBuilder.DropTable(
                name: "BlockStagUserWhitelist");

            migrationBuilder.DropTable(
                name: "TermStagConnection");

            migrationBuilder.DropTable(
                name: "UserPasswordRecovery");

            migrationBuilder.DropTable(
                name: "BlockAction");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Block");

            migrationBuilder.DropTable(
                name: "SubjectInYearTerm");

            migrationBuilder.DropTable(
                name: "SubjectInYear");

            migrationBuilder.DropTable(
                name: "Subject");
        }
    }
}
