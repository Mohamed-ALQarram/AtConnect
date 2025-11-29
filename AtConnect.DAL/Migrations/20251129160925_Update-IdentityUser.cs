using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtConnect.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResetTokenExpires",
                table: "Users",
                newName: "VerifyTokenExpires");

            migrationBuilder.RenameColumn(
                name: "PasswordResetToken",
                table: "Users",
                newName: "VerifyToken");

            migrationBuilder.AddColumn<bool>(
                name: "isEmailVerified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isEmailVerified",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "VerifyTokenExpires",
                table: "Users",
                newName: "ResetTokenExpires");

            migrationBuilder.RenameColumn(
                name: "VerifyToken",
                table: "Users",
                newName: "PasswordResetToken");
        }
    }
}
