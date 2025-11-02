using EmployeeManagement.Application.DTOs.Requests;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

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

