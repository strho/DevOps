using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugService.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedTo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedTo",
                table: "Bugs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Bugs");
        }
    }
}
