using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankUsers_Users_UserId",
                table: "BankUsers");

            migrationBuilder.DropIndex(
                name: "IX_BankUsers_UserId",
                table: "BankUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BankUsers");

            migrationBuilder.AddColumn<int>(
                name: "BankUserId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BankUserId",
                table: "Users",
                column: "BankUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_BankUsers_BankUserId",
                table: "Users",
                column: "BankUserId",
                principalTable: "BankUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_BankUsers_BankUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BankUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BankUserId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "BankUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankUsers_UserId",
                table: "BankUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankUsers_Users_UserId",
                table: "BankUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
