using AutoMapper;
using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.ValueObjects;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
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
            null); // No manager on registration

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

        // Generate token
        var token = _jwtTokenGenerator.GenerateToken(createdEmployee);
        var expiresAt = _jwtTokenGenerator.GetExpirationDate();

        return new AuthResponse
        {
            Token = token,
            User = _mapper.Map<EmployeeResponse>(createdEmployee),
            ExpiresAt = expiresAt
        };
    }
}

