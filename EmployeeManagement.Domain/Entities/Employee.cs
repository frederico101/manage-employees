namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocNumber { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new();
    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }
    public string Role { get; set; } = "Employee";
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

