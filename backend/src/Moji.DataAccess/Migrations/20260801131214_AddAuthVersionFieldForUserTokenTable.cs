using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moji.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthVersionFieldForUserTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthVersion",
                table: "UserTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthVersion",
                table: "UserTokens");
        }
    }
}
