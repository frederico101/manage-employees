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

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CreateEmployeeCommandHandler _handler;
    private readonly AutoMapper.IMapper _mapper;

    public CreateEmployeeCommandHandlerTests()
    {
        _repositoryMock = new Mock<IEmployeeRepository>();
        _permissionServiceMock = new Mock<IPermissionService>();
        _passwordHasherMock = new Mock<IPasswordHasher>();

        // Setup AutoMapper
        var config = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EmployeeManagement.Application.Mappings.MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new CreateEmployeeCommandHandler(
            _repositoryMock.Object,
            _permissionServiceMock.Object,
            _passwordHasherMock.Object,
            _mapper);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldCreateEmployee()
    {
        // Arrange
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

        var command = new CreateEmployeeCommand
        {
            Request = request,
            CurrentUserId = 1,
            CurrentUserRole = EmployeeRole.Director
        };

        _permissionServiceMock.Setup(p => p.CanCreateRole(EmployeeRole.Director, EmployeeRole.Employee))
            .Returns(true);
        _repositoryMock.Setup(r => r.EmailExistsAsync("john.doe@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.DocNumberExistsAsync("123456789", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasherMock.Setup(h => h.HashPassword("Password123!"))
            .Returns("hashedPassword123");

        Employee? capturedEmployee = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee emp, CancellationToken ct) =>
            {
                capturedEmployee = emp;
                return emp;
            });
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken ct) => capturedEmployee!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john.doe@example.com");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithHigherRole_ShouldThrowUnauthorizedException()
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
            Role = EmployeeRole.Director
        };

        var command = new CreateEmployeeCommand
        {
            Request = request,
            CurrentUserId = 1,
            CurrentUserRole = EmployeeRole.Employee // Employee trying to create Director
        };

        _permissionServiceMock.Setup(p => p.CanCreateRole(EmployeeRole.Employee, EmployeeRole.Director))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "existing@example.com",
            DocNumber = "123456789",
            Password = "Password123!",
            DateOfBirth = DateTime.Today.AddYears(-25),
            Phones = new List<PhoneRequest>
            {
                new PhoneRequest { Number = "1234567890", Type = PhoneType.Mobile }
            },
            Role = EmployeeRole.Employee
        };

        var command = new CreateEmployeeCommand
        {
            Request = request,
            CurrentUserId = 1,
            CurrentUserRole = EmployeeRole.Director
        };

        _permissionServiceMock.Setup(p => p.CanCreateRole(It.IsAny<EmployeeRole>(), It.IsAny<EmployeeRole>()))
            .Returns(true);
        _repositoryMock.Setup(r => r.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(command, CancellationToken.None));

        exception.Message.Should().Contain("already exists");
    }
}

