using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.UseCases.Employees.Queries;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Application.UseCases;

public class ListEmployeesQueryHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly ListEmployeesQueryHandler _handler;
    private readonly AutoMapper.IMapper _mapper;

    public ListEmployeesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();

        var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeManagement.Application.Mappings.MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new ListEmployeesQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_WithNoFilters_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            CreateEmployee("John", "Doe", "john@example.com", EmployeeRole.Employee),
            CreateEmployee("Jane", "Smith", "jane@example.com", EmployeeRole.Leader)
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var query = new ListEmployeesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ShouldFilterEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            CreateEmployee("John", "Doe", "john@example.com", EmployeeRole.Employee),
            CreateEmployee("Jane", "Smith", "jane@example.com", EmployeeRole.Leader),
            CreateEmployee("Bob", "Johnson", "bob@example.com", EmployeeRole.Employee)
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var query = new ListEmployeesQuery { SearchTerm = "John" };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2); // John Doe and Bob Johnson
        result.All(e => e.FirstName.Contains("John", StringComparison.OrdinalIgnoreCase) ||
                        e.LastName.Contains("John", StringComparison.OrdinalIgnoreCase) ||
                        e.Email.Contains("John", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithRoleFilter_ShouldFilterByRole()
    {
        // Arrange
        var employees = new List<Employee>
        {
            CreateEmployee("John", "Doe", "john@example.com", EmployeeRole.Employee),
            CreateEmployee("Jane", "Smith", "jane@example.com", EmployeeRole.Leader),
            CreateEmployee("Bob", "Johnson", "bob@example.com", EmployeeRole.Employee)
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var query = new ListEmployeesQuery { Role = (int)EmployeeRole.Employee };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.All(e => e.Role == EmployeeRole.Employee).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldReturnLimitedResults()
    {
        // Arrange
        var employees = Enumerable.Range(1, 20)
            .Select(i => CreateEmployee($"Employee{i}", "Test", $"emp{i}@example.com", EmployeeRole.Employee))
            .ToList();

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var query = new ListEmployeesQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(10);
    }

    private Employee CreateEmployee(string firstName, string lastName, string email, EmployeeRole role)
    {
        var emailVo = Email.Create(email);
        return new Employee(
            firstName,
            lastName,
            emailVo,
            Guid.NewGuid().ToString(),
            DateTime.Today.AddYears(-25),
            role,
            "hashedPassword",
            null);
    }
}

