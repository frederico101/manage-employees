using EmployeeManagement.Application.DTOs.Responses;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.UseCases.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IEmployeeRepository employeeRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _employeeRepository = employeeRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (employee == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(request.Request.Password, employee.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var token = _jwtTokenGenerator.GenerateToken(employee);
        var expiresAt = _jwtTokenGenerator.GetExpirationDate();

        return new AuthResponse
        {
            Token = token,
            User = new EmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.EmailAddress,
                DocNumber = employee.DocNumber,
                Role = employee.Role,
                DateOfBirth = employee.DateOfBirth,
                Age = employee.GetAge(),
                CreatedAt = employee.CreatedAt,
                Phones = employee.Phones.Select(p => new PhoneResponse
                {
                    Number = p.Number,
                    Type = p.Type
                }).ToList()
            },
            ExpiresAt = expiresAt
        };
    }
}

