using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Services;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Tests.Domain.Services;

public class PermissionServiceTests
{
    private readonly PermissionService _permissionService;

    public PermissionServiceTests()
    {
        _permissionService = new PermissionService();
    }

    [Fact]
    public void CanCreateRole_EmployeeCreatingEmployee_ShouldReturnTrue()
    {
        // Act
        var result = _permissionService.CanCreateRole(EmployeeRole.Employee, EmployeeRole.Employee);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanCreateRole_EmployeeCreatingLeader_ShouldReturnFalse()
    {
        // Act
        var result = _permissionService.CanCreateRole(EmployeeRole.Employee, EmployeeRole.Leader);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanCreateRole_LeaderCreatingEmployee_ShouldReturnTrue()
    {
        // Act
        var result = _permissionService.CanCreateRole(EmployeeRole.Leader, EmployeeRole.Employee);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanUpdateEmployee_DirectorUpdatingEmployee_ShouldReturnTrue()
    {
        // Act
        var result = _permissionService.CanUpdateEmployee(EmployeeRole.Director, EmployeeRole.Employee);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanUpdateEmployee_EmployeeUpdatingEmployee_ShouldReturnFalse()
    {
        // Act
        var result = _permissionService.CanUpdateEmployee(EmployeeRole.Employee, EmployeeRole.Employee);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanDeleteEmployee_DirectorDeletingLeader_ShouldReturnTrue()
    {
        // Act
        var result = _permissionService.CanDeleteEmployee(EmployeeRole.Director, EmployeeRole.Leader);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanDeleteEmployee_LeaderDeletingLeader_ShouldReturnFalse()
    {
        // Act
        var result = _permissionService.CanDeleteEmployee(EmployeeRole.Leader, EmployeeRole.Leader);

        // Assert
        result.Should().BeFalse();
    }
}

