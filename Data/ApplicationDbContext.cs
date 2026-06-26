using Microsoft.EntityFrameworkCore;
using BugTrack.Models;

namespace BugTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bug> Bugs { get; set; }
        public DbSet<TestCase> TestCases { get; set; }
        public DbSet<TestRun> TestRuns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TestRun>()
                .HasOne(tr => tr.TestCase)
                .WithMany(tc => tc.TestRuns)
                .HasForeignKey(tr => tr.TestCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestRun>()
                .HasOne(tr => tr.LinkedBug)
                .WithMany(b => b.RelatedTestRuns)
                .HasForeignKey(tr => tr.LinkedBugId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bug>()
                .HasOne(b => b.LinkedTestRun)
                .WithMany()
                .HasForeignKey(b => b.LinkedTestRunId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}