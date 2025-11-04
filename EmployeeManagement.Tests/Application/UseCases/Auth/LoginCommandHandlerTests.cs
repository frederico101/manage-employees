using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.UseCases.Auth.Commands;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Application.UseCases.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();

        _handler = new LoginCommandHandler(
            _repositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnAuthResponse()
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

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var command = new LoginCommand { Request = request };

        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        _passwordHasherMock.Setup(h => h.VerifyPassword("Password123!", "hashedPassword"))
            .Returns(true);
        _jwtTokenGeneratorMock.Setup(j => j.GenerateToken(employee))
            .Returns("test-jwt-token");
        _jwtTokenGeneratorMock.Setup(j => j.GetExpirationDate())
            .Returns(DateTime.UtcNow.AddHours(24));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test-jwt-token");
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Handle_WithInvalidEmail_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        var command = new LoginCommand { Request = request };

        _repositoryMock.Setup(r => r.GetByEmailAsync("nonexistent@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldThrowUnauthorizedException()
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

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        var command = new LoginCommand { Request = request };

        _repositoryMock.Setup(r => r.GetByEmailAsync("test@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        _passwordHasherMock.Setup(h => h.VerifyPassword("WrongPassword123!", "hashedPassword"))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

