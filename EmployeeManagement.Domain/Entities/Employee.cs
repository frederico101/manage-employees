using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;

namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string EmailAddress { get; private set; } = string.Empty; // Stored as string for EF Core
    public string DocNumber { get; private set; } = string.Empty;
    public List<Phone> Phones { get; private set; } = new();
    public int? ManagerId { get; private set; }
    public Employee? Manager { get; private set; }
    public EmployeeRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation property for employees managed by this employee
    // Note: This will be configured in DbContext to avoid circular reference issues

    // Private constructor for EF Core
    private Employee() { }

    public Employee(
        string firstName,
        string lastName,
        Email email,
        string docNumber,
        DateTime dateOfBirth,
        EmployeeRole role,
        string passwordHash,
        int? managerId = null)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateDocNumber(docNumber);
        ValidateDateOfBirth(dateOfBirth);

        FirstName = firstName;
        LastName = lastName;
        EmailAddress = email.Value;
        DocNumber = docNumber;
        DateOfBirth = dateOfBirth;
        Role = role;
        PasswordHash = passwordHash;
        ManagerId = managerId;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddPhone(Phone phone)
    {
        if (phone == null)
            throw new ArgumentNullException(nameof(phone));

        if (Phones.Contains(phone))
            throw new InvalidOperationException("Phone number already exists");

        Phones.Add(phone);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemovePhone(Phone phone)
    {
        if (phone == null)
            throw new ArgumentNullException(nameof(phone));

        if (!Phones.Remove(phone))
            throw new InvalidOperationException("Phone number not found");

        if (Phones.Count == 0)
            throw new InvalidOperationException("Employee must have at least one phone number");

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePersonalInfo(string firstName, string lastName, DateTime dateOfBirth)
    {
        ValidateFirstName(firstName);
        ValidateLastName(lastName);
        ValidateDateOfBirth(dateOfBirth);

        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEmail(Email email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        EmailAddress = email.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeManager(int? managerId)
    {
        if (managerId.HasValue && managerId.Value == Id)
            throw new InvalidOperationException("Employee cannot be their own manager");

        ManagerId = managerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(EmployeeRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanCreateRole(EmployeeRole targetRole)
    {
        return Role >= targetRole;
    }

    public bool IsAdult()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age >= 18;
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age)) age--;
        return age;
    }

    public Email GetEmail() => Email.Create(EmailAddress);

    private static void ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));

        if (firstName.Length > 100)
            throw new ArgumentException("First name cannot exceed 100 characters", nameof(firstName));
    }

    private static void ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        if (lastName.Length > 100)
            throw new ArgumentException("Last name cannot exceed 100 characters", nameof(lastName));
    }

    private static void ValidateDocNumber(string docNumber)
    {
        if (string.IsNullOrWhiteSpace(docNumber))
            throw new ArgumentException("Document number is required", nameof(docNumber));

        if (docNumber.Length > 50)
            throw new ArgumentException("Document number cannot exceed 50 characters", nameof(docNumber));
    }

    private static void ValidateDateOfBirth(DateTime dateOfBirth)
    {
        if (dateOfBirth > DateTime.Today)
            throw new ArgumentException("Date of birth cannot be in the future", nameof(dateOfBirth));

        var today = DateTime.Today;
        var age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age)) age--;

        if (age < 18)
            throw new ArgumentException("Employee must be at least 18 years old", nameof(dateOfBirth));
    }
}
