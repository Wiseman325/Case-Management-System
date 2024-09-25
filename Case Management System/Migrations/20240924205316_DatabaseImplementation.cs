using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseImplementation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "AspNetUsers",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "casesType",
                columns: table => new
                {
                    CaseTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_casesType", x => x.CaseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    CaseNum = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IncidentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IncidentTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReported = table.Column<DateOnly>(type: "date", nullable: false),
                    CaseTypeId = table.Column<int>(type: "int", nullable: false),
                    CitizenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfficerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.CaseNum);
                    table.ForeignKey(
                        name: "FK_cases_AspNetUsers_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_cases_AspNetUsers_OfficerId",
                        column: x => x.OfficerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cases_casesType_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "casesType",
                        principalColumn: "CaseTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "citizenCases",
                columns: table => new
                {
                    CitizenCaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CaseNum = table.Column<int>(type: "int", nullable: false),
                    DateReported = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_citizenCases", x => x.CitizenCaseId);
                    table.ForeignKey(
                        name: "FK_citizenCases_AspNetUsers_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_citizenCases_cases_CaseNum",
                        column: x => x.CaseNum,
                        principalTable: "cases",
                        principalColumn: "CaseNum",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cases_CaseTypeId",
                table: "cases",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CitizenId",
                table: "cases",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_OfficerId",
                table: "cases",
                column: "OfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_citizenCases_CaseNum",
                table: "citizenCases",
                column: "CaseNum");

            migrationBuilder.CreateIndex(
                name: "IX_citizenCases_CitizenId",
                table: "citizenCases",
                column: "CitizenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "citizenCases");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "casesType");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4,
                oldNullable: true);
        }
    }
}
