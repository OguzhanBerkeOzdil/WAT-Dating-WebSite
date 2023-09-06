using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappproject.Migrations
{
    /// <inheritdoc />
    public partial class jadfsas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RolId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RolId",
                table: "Users");
        }
    }
}
