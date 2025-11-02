using EmployeeManagement.Domain.Enums;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class DeleteEmployeeCommand : IRequest<Unit>
{
    public int EmployeeId { get; set; }
    public int CurrentUserId { get; set; }
    public EmployeeRole CurrentUserRole { get; set; }
}

