using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Services;

public interface IPermissionService
{
    bool CanCreateRole(EmployeeRole currentUserRole, EmployeeRole targetRole);
    bool CanUpdateEmployee(EmployeeRole currentUserRole, EmployeeRole targetEmployeeRole);
    bool CanDeleteEmployee(EmployeeRole currentUserRole, EmployeeRole targetEmployeeRole);
    bool HasHigherOrEqualRole(EmployeeRole role1, EmployeeRole role2);
}

