using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtConnect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNotificationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatRequestId",
                table: "Notifications",
                type: "int",
                nullable: true);


            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ChatRequests_ChatRequestId",
                table: "Notifications",
                column: "ChatRequestId",
                principalTable: "ChatRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Chats_ChatId",
                table: "Notifications",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ChatRequests_ChatRequestId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Chats_ChatId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ChatRequestId",
                table: "Notifications");
        }
    }
}
