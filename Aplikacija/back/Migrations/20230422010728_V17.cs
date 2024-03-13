using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back.Migrations
{
    /// <inheritdoc />
    public partial class V17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers");

            migrationBuilder.DropColumn(
                name: "QuizID",
                table: "QuizzesUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizzesID",
                table: "QuizzesUsers",
                column: "QuizzesID",
                principalTable: "Quizzes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizzesID",
                table: "QuizzesUsers");

            migrationBuilder.AddColumn<int>(
                name: "QuizID",
                table: "QuizzesUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizzesUsers_QuizID",
                table: "QuizzesUsers",
                column: "QuizID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizzesUsers_Quizzes_QuizID",
                table: "QuizzesUsers",
                column: "QuizID",
                principalTable: "Quizzes",
                principalColumn: "ID");
        }
    }
}
