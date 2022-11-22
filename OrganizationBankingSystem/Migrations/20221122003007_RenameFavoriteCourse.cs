using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganizationBankingSystem.Migrations
{
    /// <inheritdoc />
    public partial class RenameFavoriteCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteCourse_BankUsers_BankUserId",
                table: "FavoriteCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteCourse",
                table: "FavoriteCourse");

            migrationBuilder.RenameTable(
                name: "FavoriteCourse",
                newName: "FavoriteCourses");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteCourse_BankUserId",
                table: "FavoriteCourses",
                newName: "IX_FavoriteCourses_BankUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteCourses",
                table: "FavoriteCourses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteCourses_BankUsers_BankUserId",
                table: "FavoriteCourses",
                column: "BankUserId",
                principalTable: "BankUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteCourses_BankUsers_BankUserId",
                table: "FavoriteCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteCourses",
                table: "FavoriteCourses");

            migrationBuilder.RenameTable(
                name: "FavoriteCourses",
                newName: "FavoriteCourse");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteCourses_BankUserId",
                table: "FavoriteCourse",
                newName: "IX_FavoriteCourse_BankUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteCourse",
                table: "FavoriteCourse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteCourse_BankUsers_BankUserId",
                table: "FavoriteCourse",
                column: "BankUserId",
                principalTable: "BankUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
