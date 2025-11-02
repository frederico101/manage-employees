using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.DocNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.DocNumber)
                .IsUnique();

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // Self-referencing relationship for Manager
            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Store Phones as JSON array or comma-separated (will be improved in Phase 2)
            entity.Property(e => e.Phones)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .HasColumnName("Phones");

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);
        });
    }
}

