using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Enums;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class CreateEmployeeCommand : IRequest<EmployeeResponse>
{
    public CreateEmployeeRequest Request { get; set; } = null!;
    public int CurrentUserId { get; set; }
    public EmployeeRole CurrentUserRole { get; set; }
}

