using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.Validators;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Application.Validators;

public class CreateEmployeeRequestValidatorTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly CreateEmployeeRequestValidator _validator;

    public CreateEmployeeRequestValidatorTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _validator = new CreateEmployeeRequestValidator(_repositoryMock.Object);
    }

    [Fact]
    public async Task Validate_WithValidRequest_ShouldPass()
    {
        // Arrange
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DocNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            },
            Role = EmployeeRole.Employee
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithEmptyFirstName_ShouldFail()
    {
        // Arrange
        var request = new CreateEmployeeRequest { FirstName = "" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Validate_WithInvalidEmail_ShouldFail()
    {
        // Arrange
        var request = new CreateEmployeeRequest { Email = "invalid-email" };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_WithDuplicateEmail_ShouldFail()
    {
        // Arrange
        _repositoryMock.Setup(r => r.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateEmployeeRequest
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("short")] // Too short
    [InlineData("nouppercase123!")] // No uppercase
    [InlineData("NOLOWERCASE123!")] // No lowercase
    [InlineData("NoDigitHere!")] // No digit
    [InlineData("NoSpecialChar123")] // No special character
    public async Task Validate_WithWeakPassword_ShouldFail(string password)
    {
        // Arrange
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DocNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocNumber = "123456789",
            Password = password,
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Validate_WithMinorDateOfBirth_ShouldFail()
    {
        // Arrange
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DocNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-17), // 17 years old
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            }
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public async Task Validate_WithNoPhones_ShouldFail()
    {
        // Arrange
        _repositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DocNumberExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>() // Empty
        };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Phones);
    }
}

