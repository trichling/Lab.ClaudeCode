using Microsoft.EntityFrameworkCore;
using LogicQuiz.Api.Models;

namespace LogicQuiz.Api.Data;

public class QuizDbContext : DbContext
{
    public QuizDbContext(DbContextOptions<QuizDbContext> options) : base(options)
    {
    }

    public DbSet<FallacyType> FallacyTypes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<GameAnswer> GameAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FallacyType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.HasIndex(e => e.Difficulty);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Statement).IsRequired().HasMaxLength(1000);
            entity.HasOne(e => e.CorrectFallacyType)
                .WithMany(f => f.Questions)
                .HasForeignKey(e => e.CorrectFallacyTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerName).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.StartTime);
        });

        modelBuilder.Entity<GameAnswer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.GameSession)
                .WithMany(g => g.Answers)
                .HasForeignKey(e => e.GameSessionId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Question)
                .WithMany()
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.SelectedFallacyType)
                .WithMany()
                .HasForeignKey(e => e.SelectedFallacyTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
