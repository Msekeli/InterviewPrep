using InterviewPrep.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewPrep.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<InterviewSession> InterviewSessions => Set<InterviewSession>();
    public DbSet<InterviewQuestion> InterviewQuestions => Set<InterviewQuestion>();
    public DbSet<InterviewAnswer> InterviewAnswers => Set<InterviewAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);
        });

        modelBuilder.Entity<InterviewSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CvText).IsRequired();
            entity.Property(x => x.JobSpecText).IsRequired();
            entity.Property(x => x.CompanyText).IsRequired();
            entity.Property(x => x.Feedback).IsRequired();
        });

        modelBuilder.Entity<InterviewQuestion>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).IsRequired();
        });

        modelBuilder.Entity<InterviewAnswer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Transcript).IsRequired();
        });
    }
}