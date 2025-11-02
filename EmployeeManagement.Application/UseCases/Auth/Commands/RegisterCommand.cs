using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Auth.Commands;

public class RegisterCommand : IRequest<AuthResponse>
{
    public RegisterRequest Request { get; set; } = null!;
}

