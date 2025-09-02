using AnaBank.Accounts.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace AnaBank.Accounts.UnitTests.Domain.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("11144477735")]
    [InlineData("111.444.777-35")]
    [InlineData("52998224725")]
    public void Constructor_ValidCpf_ShouldCreateInstance(string cpfValue)
    {
        var cpf = new Cpf(cpfValue);

        cpf.Value.Should().Be(cpfValue.Replace(".", "").Replace("-", ""));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("123")]
    [InlineData("123456789012345")]
    public void Constructor_InvalidCpf_ShouldThrowException(string invalidCpf)
    {
        FluentActions.Invoking(() => new Cpf(invalidCpf))
            .Should().Throw<ArgumentException>()
            .WithMessage("*CPF*");
    }

    [Fact]
    public void ToString_ShouldReturnCpfValue()
    {
        var cpf = new Cpf("11144477735");

        var result = cpf.ToString();

        result.Should().Be("11144477735");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        Cpf cpf = "11144477735";

        cpf.Value.Should().Be("11144477735");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        var cpf = new Cpf("11144477735");

        string cpfString = cpf;

        cpfString.Should().Be("11144477735");
    }
}