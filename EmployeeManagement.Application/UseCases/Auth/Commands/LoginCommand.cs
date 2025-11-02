using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Auth.Commands;

public class LoginCommand : IRequest<AuthResponse>
{
    public LoginRequest Request { get; set; } = null!;
}

