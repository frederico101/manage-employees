using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Specifications;

public class EmployeeByEmailSpecification : BaseSpecification<Employee>
{
    public EmployeeByEmailSpecification(string email)
        : base(e => e.EmailAddress.ToLower() == email.ToLower())
    {
    }
}

public class EmployeeByDocNumberSpecification : BaseSpecification<Employee>
{
    public EmployeeByDocNumberSpecification(string docNumber)
        : base(e => e.DocNumber == docNumber)
    {
    }
}

public class EmployeeByRoleSpecification : BaseSpecification<Employee>
{
    public EmployeeByRoleSpecification(EmployeeRole role)
        : base(e => e.Role == role)
    {
    }
}

public class EmployeeByManagerSpecification : BaseSpecification<Employee>
{
    public EmployeeByManagerSpecification(int managerId)
        : base(e => e.ManagerId == managerId)
    {
        AddInclude(e => e.Manager!);
    }
}

public class EmployeesWithManagersSpecification : BaseSpecification<Employee>
{
    public EmployeesWithManagersSpecification()
        : base(e => true)
    {
        AddInclude(e => e.Manager!);
    }
}

