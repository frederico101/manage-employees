using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Services;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPermissionService _permissionService;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IPermissionService permissionService)
    {
        _employeeRepository = employeeRepository;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
        }

        // Check permissions - user cannot delete employees with higher/equal role
        if (!_permissionService.CanDeleteEmployee(request.CurrentUserRole, employee.Role))
        {
            throw new UnauthorizedAccessException($"You cannot delete an employee with role {employee.Role}. Your role: {request.CurrentUserRole}");
        }

        // Check if employee has managed employees
        var managedEmployees = await _employeeRepository.GetByManagerIdAsync(request.EmployeeId, cancellationToken);
        if (managedEmployees.Any())
        {
            throw new InvalidOperationException("Cannot delete employee who has managed employees. Reassign them first.");
        }

        await _employeeRepository.DeleteAsync(employee, cancellationToken);
        return Unit.Value;
    }
}

