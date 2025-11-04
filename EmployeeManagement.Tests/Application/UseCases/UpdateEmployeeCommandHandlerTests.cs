using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Application.UseCases.Employees.Commands;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Application.UseCases;

public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly UpdateEmployeeCommandHandler _handler;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateEmployeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _permissionServiceMock = new Mock<IPermissionService>();

        var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeManagement.Application.Mappings.MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new UpdateEmployeeCommandHandler(
            _repositoryMock.Object,
            _permissionServiceMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateEmployee()
    {
        // Arrange
        var email = Email.Create("john@example.com");
        var employee = new Employee(
            "John",
            "Doe",
            email,
            "123456789",
            DateTime.Today.AddYears(-25),
            EmployeeRole.Employee,
            "hashedPassword",
            null);
        
        // Set Id using reflection (normally set by EF Core)
        var idProperty = typeof(Employee).GetProperty("Id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        if (idProperty != null && idProperty.CanWrite)
        {
            idProperty.SetValue(employee, 1);
        }
        
        // Add initial phone so employee is valid
        var initialPhone = Phone.Create("1234567890", PhoneType.Mobile);
        employee.AddPhone(initialPhone);

        var request = new UpdateEmployeeRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            DateOfBirth = DateTime.Today.AddYears(-30),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "9876543210", Type = PhoneType.Mobile }
            }
        };

        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 1,
            Request = request,
            CurrentUserId = 2,
            CurrentUserRole = EmployeeRole.Director
        };

        // Setup GetByIdAsync to return employee (called twice - before and after update)
        // The handler modifies the employee in place, so returning the same instance should work
        _repositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        _permissionServiceMock.Setup(p => p.CanUpdateEmployee(EmployeeRole.Director, EmployeeRole.Employee))
            .Returns(true);
        _repositoryMock.Setup(r => r.EmailExistsAsync("jane@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmployee_ShouldThrowException()
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
                new PhoneRequest { Number = "9876543210", Type = PhoneType.Mobile }
            }
        };

        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 999,
            Request = request,
            CurrentUserId = 2,
            CurrentUserRole = EmployeeRole.Director
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }
}

