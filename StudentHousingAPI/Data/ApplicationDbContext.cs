using Microsoft.EntityFrameworkCore;
using StudentHousingAPI.Models;

namespace StudentHousingAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<CleaningTask> CleaningTasks => Set<CleaningTask>();
    public DbSet<GarbageTask> GarbageTasks => Set<GarbageTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Building)
                .WithMany(b => b.Residents)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Building configuration
        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
        });

        // Issue configuration
        modelBuilder.Entity<Issue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SharedSpace).HasMaxLength(200);
            
            entity.HasOne(e => e.CreatedByUser)
                .WithMany(u => u.CreatedIssues)
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Building)
                .WithMany(b => b.Issues)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CleaningTask configuration
        modelBuilder.Entity<CleaningTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.SharedSpace).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Building)
                .WithMany(b => b.CleaningTasks)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.AssignedUser)
                .WithMany(u => u.AssignedCleaningTasks)
                .HasForeignKey(e => e.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // GarbageTask configuration
        modelBuilder.Entity<GarbageTask>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            
            entity.HasOne(e => e.Building)
                .WithMany(b => b.GarbageTasks)
                .HasForeignKey(e => e.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.AssignedUser)
                .WithMany(u => u.AssignedGarbageTasks)
                .HasForeignKey(e => e.AssignedUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
