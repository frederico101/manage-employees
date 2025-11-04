using System.Security.Claims;
using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.UseCases.Employees.Commands;
using EmployeeManagement.Application.UseCases.Employees.Queries;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.API.Controllers;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.API;

public class EmployeesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<EmployeesController>> _loggerMock;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<EmployeesController>>();
        _controller = new EmployeesController(_mediatorMock.Object, _loggerMock.Object);

        // Setup HttpContext with user claims
        var claims = new List<Claim>
        {
            new Claim("EmployeeId", "1"),
            new Claim("Role", "Director")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetEmployees_ShouldReturnListOfEmployees()
    {
        // Arrange
        var employees = new List<EmployeeResponse>
        {
            new EmployeeResponse
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Role = EmployeeRole.Employee
            }
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<ListEmployeesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        // Act
        var result = await _controller.GetEmployees(null, null, null, 1, 10);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedEmployees = okResult!.Value as List<EmployeeResponse>;
        returnedEmployees.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetEmployee_WithValidId_ShouldReturnEmployee()
    {
        // Arrange
        var employee = new EmployeeResponse
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Role = EmployeeRole.Employee
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<GetEmployeeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        // Act
        var result = await _controller.GetEmployee(1);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedEmployee = okResult!.Value as EmployeeResponse;
        returnedEmployee!.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateEmployee_WithValidRequest_ShouldReturnCreatedEmployee()
    {
        // Arrange
        var request = new CreateEmployeeRequest
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

        var employeeResponse = new EmployeeResponse
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Role = EmployeeRole.Employee
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeResponse);

        // Act
        var result = await _controller.CreateEmployee(request);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task UpdateEmployee_WithValidRequest_ShouldReturnUpdatedEmployee()
    {
        // Arrange
        var request = new UpdateEmployeeRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            DateOfBirth = DateTime.Today.AddYears(-30),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            }
        };

        var employeeResponse = new EmployeeResponse
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com"
        };

        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeResponse);

        // Act
        var result = await _controller.UpdateEmployee(1, request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedEmployee = okResult!.Value as EmployeeResponse;
        returnedEmployee!.FirstName.Should().Be("Jane");
    }

    [Fact]
    public async Task DeleteEmployee_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteEmployeeCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MediatR.Unit.Value);

        // Act
        var result = await _controller.DeleteEmployee(1);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }
}

