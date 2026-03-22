using EduTests.Database.Entities;
using EduTests.Database.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace EduTests.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<AnonymousUser> AnonymousUsers { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<TestCompletion> TestCompletions { get; set; }
    public DbSet<UserAnswer> UserAnswers { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<BannedUser> BannedUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TestConfiguration());
        modelBuilder.ApplyConfiguration(new TestResultConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new TestCompletionConfiguration());
        modelBuilder.ApplyConfiguration(new UserAnswerConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new ReportConfiguration());
        modelBuilder.ApplyConfiguration(new BannedUserConfiguration());
        modelBuilder.ApplyConfiguration(new UserRatingConfiguration());
    }
}