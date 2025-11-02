using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs.Requests;

public class UpdateEmployeeRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<PhoneRequest> Phones { get; set; } = new();
    public int? ManagerId { get; set; }
    public DateTime DateOfBirth { get; set; }
}

