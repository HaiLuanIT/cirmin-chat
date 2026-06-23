using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moji.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifyFriendshipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Friendships",
                newName: "UserRightId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendShips_ReceiverId_Status_UpdatedAt_Desc",
                table: "Friendships",
                newName: "IX_FriendShips_UserRightId_Status_UpdatedAt_Desc");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Pending");

            migrationBuilder.AddColumn<Guid>(
                name: "UserLeftId",
                table: "Friendships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserLeftId_UserRightId",
                table: "Friendships",
                columns: new[] { "UserLeftId", "UserRightId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_UserLeftId",
                table: "Friendships",
                column: "UserLeftId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_UserRightId",
                table: "Friendships",
                column: "UserRightId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_UserLeftId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_UserRightId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserLeftId_UserRightId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserLeftId",
                table: "Friendships");

            migrationBuilder.RenameColumn(
                name: "UserRightId",
                table: "Friendships",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_FriendShips_UserRightId_Status_UpdatedAt_Desc",
                table: "Friendships",
                newName: "IX_FriendShips_ReceiverId_Status_UpdatedAt_Desc");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
