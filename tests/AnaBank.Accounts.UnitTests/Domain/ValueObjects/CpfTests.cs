using AnaBank.Accounts.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AnaBank.Accounts.UnitTests.Domain.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("12345678909")]
    [InlineData("123.456.789-09")]
    [InlineData("11144477735")]
    public void Constructor_ValidCpf_ShouldCreateInstance(string cpfValue)
    {
        // Act
        var cpf = new Cpf(cpfValue);

        // Assert
        cpf.Value.Should().Be(cpfValue.Replace(".", "").Replace("-", ""));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("12345678900")] // CPF inválido
    [InlineData("11111111111")] // Todos os dígitos iguais
    [InlineData("123")]         // Muito curto
    [InlineData("123456789012345")] // Muito longo
    public void Constructor_InvalidCpf_ShouldThrowException(string invalidCpf)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Cpf(invalidCpf))
            .Should().Throw<ArgumentException>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public void ToString_ShouldReturnCpfValue()
    {
        // Arrange
        var cpf = new Cpf("12345678909");

        // Act
        var result = cpf.ToString();

        // Assert
        result.Should().Be("12345678909");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        // Act
        Cpf cpf = "12345678909";

        // Assert
        cpf.Value.Should().Be("12345678909");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        // Arrange
        var cpf = new Cpf("12345678909");

        // Act
        string cpfString = cpf;

        // Assert
        cpfString.Should().Be("12345678909");
    }
}