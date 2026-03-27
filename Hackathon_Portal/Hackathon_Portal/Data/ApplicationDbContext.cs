using Hackathon_Portal.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hackathon_Portal.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<Score> Scores => Set<Score>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EventRegistration>()
            .HasKey(er => new { er.EventId, er.TeamId });

        modelBuilder.Entity<EventRegistration>()
            .HasOne(er => er.Event)
            .WithMany(e => e.EventRegistrations)
            .HasForeignKey(er => er.EventId);

        modelBuilder.Entity<EventRegistration>()
            .HasOne(er => er.Team)
            .WithMany(t => t.EventRegistrations)
            .HasForeignKey(er => er.TeamId);
    }
}