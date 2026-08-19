using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CirMin.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthVersionFieldForUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthVersion",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthVersion",
                table: "Users");
        }
    }
}
