using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.ValueObjects;

public class Phone
{
    public string Number { get; private set; }
    public PhoneType Type { get; private set; }

    private Phone(string number, PhoneType type)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(number));

        // Remove common formatting characters
        var cleaned = number.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("+", "");

        if (cleaned.Length < 8 || cleaned.Length > 15)
            throw new ArgumentException("Phone number must be between 8 and 15 digits", nameof(number));

        Number = cleaned;
        Type = type;
    }

    public static Phone Create(string number, PhoneType type = PhoneType.Mobile)
    {
        return new Phone(number, type);
    }

    public override string ToString() => Number;

    public override bool Equals(object? obj)
    {
        if (obj is Phone other)
            return Number == other.Number && Type == other.Type;
        return false;
    }

    public override int GetHashCode() => HashCode.Combine(Number, Type);
}

