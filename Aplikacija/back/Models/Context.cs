using Microsoft.EntityFrameworkCore;

namespace Models;

public class Context : DbContext
{
    public Context(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Answer>? Answers { get; set; }
    public DbSet<Category>? Categorys { get; set; }
    public DbSet<Question>? Questions { get; set; }
    public DbSet<Quiz>? Quizzes { get; set; }
    public DbSet<Score>? Scores { get; set; }
    public DbSet<Users>? Users { get; set; }
    public DbSet<QuCategory>? QuestionCategorys { get; set; }

    public DbSet<QuizUser>? QuizzesUsers { get; set; }
    public DbSet<Rating>? Ratings { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuizUser>().HasKey(qu => new { qu.QuizzesID, qu.UsersID });

        modelBuilder.Entity<QuizUser>().HasOne(qu => qu.Quiz).WithMany(q => q.UsersPlayed).HasForeignKey(qu => qu.QuizzesID);

        modelBuilder.Entity<QuizUser>().HasOne(qu => qu.User).WithMany(u => u.QuizzesDone).HasForeignKey(qu => qu.UsersID);
    }
}