using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CirMin.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RefactorNamingAndAddNameForIndexOfUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserName",
                table: "Users",
                newName: "IX_Users_Username_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "Users",
                newName: "IX_Users_Email_Unique");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Username_Unique",
                table: "Users",
                newName: "IX_Users_UserName");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email_Unique",
                table: "Users",
                newName: "IX_Users_Email");
        }
    }
}
