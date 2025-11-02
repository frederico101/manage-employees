using AutoMapper;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Services;
using EmployeeManagement.Domain.ValueObjects;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Employees.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPermissionService _permissionService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IPermissionService permissionService,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _permissionService = permissionService;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<EmployeeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Check permissions - user cannot create employee with higher role
        if (!_permissionService.CanCreateRole(request.CurrentUserRole, request.Request.Role))
        {
            throw new UnauthorizedAccessException($"You cannot create an employee with role {request.Request.Role}. Your role: {request.CurrentUserRole}");
        }

        // Check if email already exists
        if (await _employeeRepository.EmailExistsAsync(request.Request.Email, cancellationToken))
        {
            throw new InvalidOperationException($"Email {request.Request.Email} already exists");
        }

        // Check if document number already exists
        if (await _employeeRepository.DocNumberExistsAsync(request.Request.DocNumber, cancellationToken))
        {
            throw new InvalidOperationException($"Document number {request.Request.DocNumber} already exists");
        }

        // Validate manager exists if provided
        if (request.Request.ManagerId.HasValue)
        {
            var manager = await _employeeRepository.GetByIdAsync(request.Request.ManagerId.Value, cancellationToken);
            if (manager == null)
            {
                throw new InvalidOperationException($"Manager with ID {request.Request.ManagerId} not found");
            }
        }

        // Hash password
        var passwordHash = _passwordHasher.HashPassword(request.Request.Password);

        // Create employee entity
        var email = Email.Create(request.Request.Email);
        var employee = new Employee(
            request.Request.FirstName,
            request.Request.LastName,
            email,
            request.Request.DocNumber,
            request.Request.DateOfBirth,
            request.Request.Role,
            passwordHash,
            request.Request.ManagerId);

        // Add phones
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

        // Save employee
        var createdEmployee = await _employeeRepository.AddAsync(employee, cancellationToken);

        // Load manager for response
        if (createdEmployee.ManagerId.HasValue)
        {
            var manager = await _employeeRepository.GetByIdAsync(createdEmployee.ManagerId.Value, cancellationToken);
            createdEmployee = await _employeeRepository.GetByIdAsync(createdEmployee.Id, cancellationToken);
        }

        return _mapper.Map<EmployeeResponse>(createdEmployee);
    }
}

