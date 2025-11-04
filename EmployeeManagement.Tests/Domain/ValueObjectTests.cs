using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EmployeeManagement.Tests.Domain;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@example.co.uk")]
    [InlineData("user+tag@example.com")]
    public void Create_WithValidEmail_ShouldCreateSuccessfully(string emailAddress)
    {
        // Act
        var email = Email.Create(emailAddress);

        // Assert
        email.Value.Should().Be(emailAddress.ToLowerInvariant());
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowException(string? emailAddress)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(emailAddress!));
    }

    [Fact]
    public void Create_WithEmail_ShouldConvertToLowercase()
    {
        // Arrange
        var emailAddress = "Test@Example.COM";

        // Act
        var email = Email.Create(emailAddress);

        // Assert
        email.Value.Should().Be("test@example.com");
    }
}

public class PhoneTests
{
    [Theory]
    [InlineData("1234567890")]
    [InlineData("123456789012345")]
    [InlineData("(123) 456-7890")]
    [InlineData("+1-234-567-8901")]
    public void Create_WithValidPhoneNumber_ShouldCreateSuccessfully(string phoneNumber)
    {
        // Act
        var phone = Phone.Create(phoneNumber, PhoneType.Mobile);

        // Assert
        phone.Number.Should().NotBeNullOrEmpty();
        phone.Type.Should().Be(PhoneType.Mobile);
    }

    [Theory]
    [InlineData("1234567")] // Too short
    [InlineData("1234567890123456")] // Too long
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidPhoneNumber_ShouldThrowException(string? phoneNumber)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Phone.Create(phoneNumber!, PhoneType.Mobile));
    }

    [Fact]
    public void Create_WithPhoneNumber_ShouldRemoveFormatting()
    {
        // Arrange
        var phoneNumber = "(123) 456-7890";

        // Act
        var phone = Phone.Create(phoneNumber, PhoneType.Mobile);

        // Assert
        phone.Number.Should().Be("1234567890");
    }
}

