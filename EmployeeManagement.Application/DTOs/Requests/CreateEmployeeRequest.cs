using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs.Requests;

public class CreateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public List<PhoneRequest> Phones { get; set; } = new();
    public int? ManagerId { get; set; }
    public EmployeeRole Role { get; set; } = EmployeeRole.Employee;
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class PhoneRequest
{
    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; } = PhoneType.Mobile;
}


