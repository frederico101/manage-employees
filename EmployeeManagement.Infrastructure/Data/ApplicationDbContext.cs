using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.ValueObjects;
using EmployeeManagement.Domain.Enums;

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

            entity.Property(e => e.EmailAddress)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(e => e.EmailAddress)
                .IsUnique();

            entity.Property(e => e.DocNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.DocNumber)
                .IsUnique();

            entity.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasConversion(
                    v => DateTime.SpecifyKind(v.Date, DateTimeKind.Utc), // Convert to UTC at midnight to preserve date
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.Property(e => e.Role)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // Self-referencing relationship for Manager
            entity.HasOne(e => e.Manager)
                .WithMany()
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Phones as owned entity collection
            entity.OwnsMany(e => e.Phones, phone =>
            {
                phone.ToTable("EmployeePhones");
                phone.Property<int>("Id");
                phone.HasKey("Id");
                phone.Property(p => p.Number)
                    .IsRequired()
                    .HasMaxLength(20);
                phone.Property(p => p.Type)
                    .IsRequired()
                    .HasConversion<int>();
            });

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasConversion(
                    v => v.ToUniversalTime(),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            entity.Property(e => e.UpdatedAt)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToUniversalTime() : (DateTime?)null,
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null);
        });
    }
}
