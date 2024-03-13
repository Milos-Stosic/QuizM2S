using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace back.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorys",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "QuestionCategorys",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionCategorys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    wantQuizMaker = table.Column<bool>(type: "bit", nullable: false),
                    wantAdmin = table.Column<bool>(type: "bit", nullable: false),
                    ProfilePictureURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfQuizzesDone = table.Column<int>(type: "int", nullable: false),
                    AccountVerified = table.Column<bool>(type: "bit", nullable: false),
                    Conf_num = table.Column<int>(type: "int", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    QuestionCategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionCategorys_QuestionCategoryID",
                        column: x => x.QuestionCategoryID,
                        principalTable: "QuestionCategorys",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pending = table.Column<bool>(type: "bit", nullable: false),
                    CurrentNumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    MaxNumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    QuizMakerID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Quizzes_Categorys_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categorys",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_QuizMakerID",
                        column: x => x.QuizMakerID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionID",
                        column: x => x.QuestionID,
                        principalTable: "Questions",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "QuestionQuiz",
                columns: table => new
                {
                    QuestionsID = table.Column<int>(type: "int", nullable: false),
                    QuizzesID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionQuiz", x => new { x.QuestionsID, x.QuizzesID });
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Questions_QuestionsID",
                        column: x => x.QuestionsID,
                        principalTable: "Questions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionQuiz_Quizzes_QuizzesID",
                        column: x => x.QuizzesID,
                        principalTable: "Quizzes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scores",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScoreValue = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuizID = table.Column<int>(type: "int", nullable: true),
                    UserIDID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scores", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Scores_Quizzes_QuizID",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Scores_Users_UserIDID",
                        column: x => x.UserIDID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionID",
                table: "Answers",
                column: "QuestionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionQuiz_QuizzesID",
                table: "QuestionQuiz",
                column: "QuizzesID");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionCategoryID",
                table: "Questions",
                column: "QuestionCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_CategoryID",
                table: "Quizzes",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_QuizMakerID",
                table: "Quizzes",
                column: "QuizMakerID");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_QuizID",
                table: "Scores",
                column: "QuizID");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_UserIDID",
                table: "Scores",
                column: "UserIDID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "QuestionQuiz");

            migrationBuilder.DropTable(
                name: "Scores");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "QuestionCategorys");

            migrationBuilder.DropTable(
                name: "Categorys");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
