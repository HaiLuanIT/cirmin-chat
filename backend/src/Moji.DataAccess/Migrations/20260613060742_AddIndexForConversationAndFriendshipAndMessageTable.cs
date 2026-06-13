using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moji.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForConversationAndFriendshipAndMessageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_RequesterId",
                table: "Friendships");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId_CreatedAt",
                table: "Messages",
                columns: new[] { "ConversationId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_FriendShips_ReceiverId_Status_UpdatedAt_Desc",
                table: "Friendships",
                columns: new[] { "ReceiverId", "Status", "UpdatedAt" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_FriendShips_RequesterId_Status_UpdatedAt_Desc",
                table: "Friendships",
                columns: new[] { "RequesterId", "Status", "UpdatedAt" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_LastMessageTime_Desc",
                table: "Conversations",
                column: "LastMessageTime",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ConversationId_CreatedAt",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_FriendShips_ReceiverId_Status_UpdatedAt_Desc",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_FriendShips_RequesterId_Status_UpdatedAt_Desc",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_LastMessageTime_Desc",
                table: "Conversations");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ConversationId",
                table: "Messages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ReceiverId",
                table: "Friendships",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_RequesterId",
                table: "Friendships",
                column: "RequesterId");
        }
    }
}
