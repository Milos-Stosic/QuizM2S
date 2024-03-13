using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back.Migrations
{
    /// <inheritdoc />
    public partial class V14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizUser_Quizzes_QuizID",
                table: "QuizUser");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizUser_Users_UserID",
                table: "QuizUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizUser",
                table: "QuizUser");

            migrationBuilder.RenameTable(
                name: "QuizUser",
                newName: "QuizzesUsers");

            migrationBuilder.RenameIndex(
                name: "IX_QuizUser_UserID",
                table: "QuizzesUsers",
                newName: "IX_QuizzesUsers_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_QuizUser_QuizID",
                table: "QuizzesUsers",
                newName: "IX_QuizzesUsers_QuizID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizzesUsers",
                table: "QuizzesUsers",
                columns: new[] { "QuizID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers",
                column: "QuizID",
                principalTable: "Quizzes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Users_UserID",
                table: "QuizzesUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Users_UserID",
                table: "QuizzesUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuizzesUsers",
                table: "QuizzesUsers");

            migrationBuilder.RenameTable(
                name: "QuizzesUsers",
                newName: "QuizUser");

            migrationBuilder.RenameIndex(
                name: "IX_QuizzesUsers_UserID",
                table: "QuizUser",
                newName: "IX_QuizUser_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizUser",
                newName: "IX_QuizUser_QuizID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuizUser",
                table: "QuizUser",
                columns: new[] { "QuizID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_QuizUser_Quizzes_QuizID",
                table: "QuizUser",
                column: "QuizID",
                principalTable: "Quizzes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizUser_Users_UserID",
                table: "QuizUser",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
