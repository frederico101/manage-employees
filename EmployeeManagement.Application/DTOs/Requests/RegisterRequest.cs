using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs.Requests;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public List<PhoneRequest> Phones { get; set; } = new();
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

