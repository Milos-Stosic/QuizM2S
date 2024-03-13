using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back.Migrations
{
    /// <inheritdoc />
    public partial class V15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Users_UserID",
                table: "QuizzesUsers");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesUsers_UserID",
                table: "QuizzesUsers");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "QuizzesUsers",
                newName: "UsersID");

            migrationBuilder.RenameColumn(
                name: "QuizID",
                table: "QuizzesUsers",
                newName: "QuizzesID");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "UsersID",
                table: "QuizzesUsers",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "QuizzesID",
                table: "QuizzesUsers",
                newName: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers",
                column: "QuizID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesUsers_UserID",
                table: "QuizzesUsers",
                column: "UserID",
                unique: true);

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
    }
}
