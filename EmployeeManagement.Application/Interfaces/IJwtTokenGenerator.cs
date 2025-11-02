using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Employee employee);
    DateTime GetExpirationDate();
}

