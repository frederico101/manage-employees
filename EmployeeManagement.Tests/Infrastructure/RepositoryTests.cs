using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EmployeeManagement.Tests.Infrastructure;

public class EmployeeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly EmployeeRepository _repository;

    public EmployeeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new EmployeeRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidEmployee_ShouldAddToDatabase()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);

        // Act
        var result = await _repository.AddAsync(employee, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        var employeeInDb = await _context.Employees.FindAsync(result.Id);
        employeeInDb.Should().NotBeNull();
        employeeInDb!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnEmployee()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        await _repository.AddAsync(employee, CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(employee.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnEmployee()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        await _repository.AddAsync(employee, CancellationToken.None);

        // Act
        var result = await _repository.GetByEmailAsync("test@example.com", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.EmailAddress.Should().Be("test@example.com");
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        await _repository.AddAsync(employee, CancellationToken.None);

        // Act
        var exists = await _repository.EmailExistsAsync("test@example.com", CancellationToken.None);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Act
        var exists = await _repository.EmailExistsAsync("nonexistent@example.com", CancellationToken.None);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedEmployee_ShouldUpdateDatabase()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        await _repository.AddAsync(employee, CancellationToken.None);

        // Act
        employee.UpdatePersonalInfo("Jane", "Smith", DateTime.Today.AddYears(-30));
        await _repository.UpdateAsync(employee, CancellationToken.None);

        // Assert
        var updated = await _repository.GetByIdAsync(employee.Id, CancellationToken.None);
        updated!.FirstName.Should().Be("Jane");
        updated.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingEmployee_ShouldRemoveFromDatabase()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        await _repository.AddAsync(employee, CancellationToken.None);
        var employeeId = employee.Id;

        // Act
        await _repository.DeleteAsync(employee, CancellationToken.None);

        // Assert
        var deleted = await _repository.GetByIdAsync(employeeId, CancellationToken.None);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

