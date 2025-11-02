using AutoMapper;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IPermissionService permissionService,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<EmployeeResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee with ID {request.EmployeeId} not found");
        }

        // Check permissions - user cannot update employees with higher/equal role
        if (!_permissionService.CanUpdateEmployee(request.CurrentUserRole, employee.Role))
        {
            throw new UnauthorizedAccessException($"You cannot update an employee with role {employee.Role}. Your role: {request.CurrentUserRole}");
        }

        // Update email if changed
        if (!string.Equals(employee.EmailAddress, request.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (await _employeeRepository.EmailExistsAsync(request.Request.Email, cancellationToken))
            {
                throw new InvalidOperationException($"Email {request.Request.Email} already exists");
            }
            employee.UpdateEmail(Email.Create(request.Request.Email));
        }

        // Update personal info
        employee.UpdatePersonalInfo(request.Request.FirstName, request.Request.LastName, request.Request.DateOfBirth);

        // Update manager if changed
        if (employee.ManagerId != request.Request.ManagerId)
        {
            if (request.Request.ManagerId.HasValue)
            {
                var manager = await _employeeRepository.GetByIdAsync(request.Request.ManagerId.Value, cancellationToken);
                if (manager == null)
                {
                    throw new InvalidOperationException($"Manager with ID {request.Request.ManagerId} not found");
                }
            }
            employee.ChangeManager(request.Request.ManagerId);
        }

        // Update phones
        employee.Phones.Clear();
        foreach (var phoneRequest in request.Request.Phones)
        {
            var phone = Phone.Create(phoneRequest.Number, phoneRequest.Type);
            employee.AddPhone(phone);
        }

        // Ensure at least one phone
        if (request.Request.Phones.Count == 0)
        {
            throw new InvalidOperationException("Employee must have at least one phone number");
        }

        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        // Load manager for response
        var updatedEmployee = await _employeeRepository.GetByIdAsync(employee.Id, cancellationToken);
        return _mapper.Map<EmployeeResponse>(updatedEmployee);
    }
}

