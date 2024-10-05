using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddEvedenceFieldOnCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Severity",
                table: "cases",
                newName: "StreetAddress");

            migrationBuilder.AddColumn<string>(
                name: "Evidence",
                table: "cases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "cases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "cases",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evidence",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "cases");

            migrationBuilder.RenameColumn(
                name: "StreetAddress",
                table: "cases",
                newName: "Severity");
        }
    }
}
