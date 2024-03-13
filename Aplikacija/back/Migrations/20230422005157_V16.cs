using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back.Migrations
{
    /// <inheritdoc />
    public partial class V16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_QuizzesUsers_UsersPlayedQuizzesID_UsersPlayedUsersID",
                table: "Quizzes");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_QuizzesUsers_QuizzesDoneQuizzesID_QuizzesDoneUsersID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_QuizzesDoneQuizzesID_QuizzesDoneUsersID",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_UsersPlayedQuizzesID_UsersPlayedUsersID",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuizzesDoneQuizzesID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuizzesDoneUsersID",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UsersPlayedQuizzesID",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "UsersPlayedUsersID",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "QuizID",
                table: "QuizzesUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesUsers_UsersID",
                table: "QuizzesUsers",
                column: "UsersID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers",
                column: "QuizID",
                principalTable: "Quizzes",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Users_UsersID",
                table: "QuizzesUsers",
                column: "UsersID",
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
                name: "FK_QuizzesUsers_Users_UsersID",
                table: "QuizzesUsers");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesUsers_UsersID",
                table: "QuizzesUsers");

            migrationBuilder.DropColumn(
                name: "QuizID",
                table: "QuizzesUsers");

            migrationBuilder.AddColumn<int>(
                name: "QuizzesDoneQuizzesID",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuizzesDoneUsersID",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsersPlayedQuizzesID",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsersPlayedUsersID",
                table: "Quizzes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_QuizzesDoneQuizzesID_QuizzesDoneUsersID",
                table: "Users",
                columns: new[] { "QuizzesDoneQuizzesID", "QuizzesDoneUsersID" });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_UsersPlayedQuizzesID_UsersPlayedUsersID",
                table: "Quizzes",
                columns: new[] { "UsersPlayedQuizzesID", "UsersPlayedUsersID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_QuizzesUsers_UsersPlayedQuizzesID_UsersPlayedUsersID",
                table: "Quizzes",
                columns: new[] { "UsersPlayedQuizzesID", "UsersPlayedUsersID" },
                principalTable: "QuizzesUsers",
                principalColumns: new[] { "QuizzesID", "UsersID" });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_QuizzesUsers_QuizzesDoneQuizzesID_QuizzesDoneUsersID",
                table: "Users",
                columns: new[] { "QuizzesDoneQuizzesID", "QuizzesDoneUsersID" },
                principalTable: "QuizzesUsers",
                principalColumns: new[] { "QuizzesID", "UsersID" });
        }
    }
}
