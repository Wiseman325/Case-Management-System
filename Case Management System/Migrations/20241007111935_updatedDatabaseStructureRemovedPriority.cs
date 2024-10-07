using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Case_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatabaseStructureRemovedPriority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "cases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "cases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
