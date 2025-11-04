using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Tests.Domain;

public class EmployeeTests
{
    [Fact]
    public void CreateEmployee_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var dateOfBirth = DateTime.Today.AddYears(-25);

        // Act
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            dateOfBirth,
            EmployeeRole.Employee,
            "hashedPassword",
            null);

        // Assert
        employee.FirstName.Should().Be("John");
        employee.LastName.Should().Be("Doe");
        employee.EmailAddress.Should().Be("test@example.com");
        employee.DocNumber.Should().Be("123456789");
        employee.Role.Should().Be(EmployeeRole.Employee);
        employee.IsAdult().Should().BeTrue();
    }

    [Fact]
    public void CreateEmployee_WithMinorDateOfBirth_ShouldThrowException()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var dateOfBirth = DateTime.Today.AddYears(-17); // 17 years old

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Employee(
                "John",
                "Doe",
                email,
                "123456789",
                dateOfBirth,
                EmployeeRole.Employee,
                "hashedPassword",
                null));

        exception.Message.Should().Contain("must be at least 18 years old");
    }

    [Fact]
    public void CreateEmployee_WithFutureDateOfBirth_ShouldThrowException()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var dateOfBirth = DateTime.Today.AddDays(1);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            new Employee(
                "John",
                "Doe",
                email,
                "123456789",
                dateOfBirth,
                EmployeeRole.Employee,
                "hashedPassword",
                null));

        exception.Message.Should().Contain("cannot be in the future");
    }

    [Fact]
    public void AddPhone_WithValidPhone_ShouldAddSuccessfully()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var phone = Phone.Create("1234567890", PhoneType.Mobile);

        // Act
        employee.AddPhone(phone);

        // Assert
        employee.Phones.Should().Contain(phone);
        employee.Phones.Count.Should().Be(1);
    }

    [Fact]
    public void AddPhone_WithDuplicatePhone_ShouldThrowException()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var phone = Phone.Create("1234567890", PhoneType.Mobile);
        employee.AddPhone(phone);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            employee.AddPhone(phone));

        exception.Message.Should().Contain("already exists");
    }

    [Fact]
    public void RemovePhone_WithLastPhone_ShouldThrowException()
    {
        // Arrange
        var employee = CreateValidEmployee();
        var phone = Phone.Create("1234567890", PhoneType.Mobile);
        employee.AddPhone(phone);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            employee.RemovePhone(phone));

        exception.Message.Should().Contain("must have at least one phone number");
    }

    [Fact]
    public void ChangeManager_WithSelfAsManager_ShouldThrowException()
    {
        // Arrange
        var employee = CreateValidEmployee();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            employee.ChangeManager(employee.Id));

        exception.Message.Should().Contain("cannot be their own manager");
    }

    [Fact]
    public void CanCreateRole_WithHigherRole_ShouldReturnFalse()
    {
        // Arrange
        var employee = CreateValidEmployee();
        employee.ChangeRole(EmployeeRole.Employee);

        // Act
        var canCreate = employee.CanCreateRole(EmployeeRole.Leader);

        // Assert
        canCreate.Should().BeFalse();
    }

    [Fact]
    public void CanCreateRole_WithEqualOrLowerRole_ShouldReturnTrue()
    {
        // Arrange
        var employee = CreateValidEmployee();
        employee.ChangeRole(EmployeeRole.Leader);

        // Act & Assert
        employee.CanCreateRole(EmployeeRole.Leader).Should().BeTrue();
        employee.CanCreateRole(EmployeeRole.Employee).Should().BeTrue();
    }

    [Fact]
    public void GetAge_WithValidDateOfBirth_ShouldReturnCorrectAge()
    {
        // Arrange
        var dateOfBirth = DateTime.Today.AddYears(-30).AddDays(-5);
        var email = Email.Create("test@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            dateOfBirth,
            EmployeeRole.Employee,
            "hashedPassword",
            null);

        // Act
        var age = employee.GetAge();

        // Assert
        age.Should().Be(30);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateEmployee_WithInvalidFirstName_ShouldThrowException(string firstName)
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var dateOfBirth = DateTime.Today.AddYears(-25);

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new Employee(
                firstName!,
                "Doe",
                email,
                "123456789",
                dateOfBirth,
                EmployeeRole.Employee,
                "hashedPassword",
                null));
    }

    private Employee CreateValidEmployee()
    {
        var email = Email.Create("test@example.com");
        var dateOfBirth = DateTime.Today.AddYears(-25);
        return new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            dateOfBirth,
            EmployeeRole.Employee,
            "hashedPassword",
            null);
    }
}

