using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusOnCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "cases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "cases");
        }
    }
}
