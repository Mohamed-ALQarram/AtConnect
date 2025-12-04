using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtConnect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addnavproptoAppUSer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "ChatRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRequests_AppUserId",
                table: "ChatRequests",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRequests_Users_AppUserId",
                table: "ChatRequests",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRequests_Users_AppUserId",
                table: "ChatRequests");

            migrationBuilder.DropIndex(
                name: "IX_ChatRequests_AppUserId",
                table: "ChatRequests");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ChatRequests");
        }
    }
}
