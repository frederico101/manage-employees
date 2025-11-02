using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs.Responses;

public class EmployeeResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public List<PhoneResponse> Phones { get; set; } = new();
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public EmployeeRole Role { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PhoneResponse
{
    public string Number { get; set; } = string.Empty;
    public PhoneType Type { get; set; }
}

