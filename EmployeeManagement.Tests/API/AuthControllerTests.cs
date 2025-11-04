using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.UseCases.Auth.Commands;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.API.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.API;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<AuthController>> _loggerMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var authResponse = new AuthResponse
        {
            Token = "test-jwt-token",
            User = new EmployeeResponse
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = EmployeeRole.Employee
            },
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as AuthResponse;
        response!.Token.Should().Be("test-jwt-token");
    }

    [Fact]
    public async Task Register_WithValidRequest_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            },
            Role = EmployeeRole.Employee
        };

        var authResponse = new AuthResponse
        {
            Token = "test-jwt-token",
            User = new EmployeeResponse
            {
                Id = 1,
                Email = "john@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = EmployeeRole.Employee
            },
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as AuthResponse;
        response!.User.FirstName.Should().Be("John");
    }
}

