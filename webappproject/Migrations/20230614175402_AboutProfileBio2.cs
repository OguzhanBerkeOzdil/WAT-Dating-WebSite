using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webappproject.Migrations
{
    /// <inheritdoc />
    public partial class AboutProfileBio2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "Users",
                newName: "Tag2");

            migrationBuilder.AddColumn<string>(
                name: "Tag1",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag1",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Tag2",
                table: "Users",
                newName: "Tags");
        }
    }
}
