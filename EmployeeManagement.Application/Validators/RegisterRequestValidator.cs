using EmployeeManagement.Application.DTOs.Requests;
using EmployeeManagement.Domain.Interfaces;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly IEmployeeRepository _employeeRepository;

    public RegisterRequestValidator(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MustAsync(async (email, cancellation) => 
                !await _employeeRepository.EmailExistsAsync(email, cancellation))
            .WithMessage("Email already exists");

        RuleFor(x => x.DocNumber)
            .NotEmpty().WithMessage("Document number is required")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters")
            .MustAsync(async (docNumber, cancellation) => 
                !await _employeeRepository.DocNumberExistsAsync(docNumber, cancellation))
            .WithMessage("Document number already exists");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit")
            .Matches(@"[!@#$%^&*(),.?\"":{}|<>]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth cannot be in the future")
            .Must(BeAtLeast18YearsOld).WithMessage("Employee must be at least 18 years old");

        RuleFor(x => x.Phones)
            .NotEmpty().WithMessage("At least one phone number is required")
            .Must(p => p.Count > 0).WithMessage("At least one phone number is required");

        RuleForEach(x => x.Phones)
            .SetValidator(new PhoneRequestValidator());
    }

    private static bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }
}

