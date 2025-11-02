namespace EmployeeManagement.Application.DTOs.Responses;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public EmployeeResponse User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

