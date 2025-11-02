using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Services;

public class PermissionService : IPermissionService
{
    public bool CanCreateRole(EmployeeRole currentUserRole, EmployeeRole targetRole)
    {
        // User cannot create a role higher than their own
        return currentUserRole >= targetRole;
    }

    public bool CanUpdateEmployee(EmployeeRole currentUserRole, EmployeeRole targetEmployeeRole)
    {
        // User cannot update employees with higher or equal role
        return currentUserRole > targetEmployeeRole;
    }

    public bool CanDeleteEmployee(EmployeeRole currentUserRole, EmployeeRole targetEmployeeRole)
    {
        // User cannot delete employees with higher or equal role
        return currentUserRole > targetEmployeeRole;
    }

    public bool HasHigherOrEqualRole(EmployeeRole role1, EmployeeRole role2)
    {
        return role1 >= role2;
    }
}

