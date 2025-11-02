using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Enums;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class UpdateEmployeeCommand : IRequest<EmployeeResponse>
{
    public int EmployeeId { get; set; }
    public UpdateEmployeeRequest Request { get; set; } = null!;
    public int CurrentUserId { get; set; }
    public EmployeeRole CurrentUserRole { get; set; }
}

