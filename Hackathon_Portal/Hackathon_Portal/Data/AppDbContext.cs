using Hackathon_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Hackathon> Hackathons { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User email unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Hackathon -> Judge
            modelBuilder.Entity<Hackathon>()
                .HasOne(h => h.Judge)
                .WithMany(u => u.JudgedHackathons)
                .HasForeignKey(h => h.JudgeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Team -> Hackathon
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Hackathon)
                .WithMany(h => h.Teams)
                .HasForeignKey(t => t.HackathonId)
                .OnDelete(DeleteBehavior.Cascade);

            // Team -> Leader
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Leader)
                .WithMany(u => u.Teams)
                .HasForeignKey(t => t.LeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            // TeamMember -> Team
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Score -> Team
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Team)
                .WithMany(t => t.Scores)
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            // Score -> Hackathon
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Hackathon)
                .WithMany(h => h.Scores)
                .HasForeignKey(s => s.HackathonId)
                .OnDelete(DeleteBehavior.NoAction);

            // Score -> Judge
            modelBuilder.Entity<Score>()
                .HasOne(s => s.Judge)
                .WithMany(u => u.Scores)
                .HasForeignKey(s => s.JudgeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Notification -> User
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Seed admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "System Admin",
                Email = "admin@hackportal.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                CreatedAt = new DateTime(2025, 1, 1)
            });
        }
    }
}
