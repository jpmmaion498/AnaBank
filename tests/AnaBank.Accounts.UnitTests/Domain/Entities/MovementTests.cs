using AnaBank.Accounts.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AnaBank.Accounts.UnitTests.Domain.Entities;

public class MovementTests
{
    [Theory]
    [InlineData("acc123", "C", 100.50)]
    [InlineData("acc456", "D", 50.25)]
    public void Constructor_ValidParameters_ShouldCreateMovement(string accountId, string type, decimal value)
    {
        // Act
        var movement = new Movement(accountId, type, value);

        // Assert
        movement.AccountId.Should().Be(accountId);
        movement.Type.Should().Be(type);
        movement.Value.Should().Be(value);
        movement.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "C", 100)]
    [InlineData("   ", "D", 100)]
    [InlineData(null, "C", 100)]
    public void Constructor_InvalidAccountId_ShouldThrowException(string accountId, string type, decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Movement(accountId, type, value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*ID da conta*");
    }

    [Theory]
    [InlineData("acc123", "", 100)]
    [InlineData("acc123", "X", 100)]
    [InlineData("acc123", null, 100)]
    public void Constructor_InvalidType_ShouldThrowException(string accountId, string type, decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Movement(accountId, type, value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*Tipo deve ser 'C' ou 'D'*");
    }

    [Theory]
    [InlineData("acc123", "C", 0)]
    [InlineData("acc123", "D", -100)]
    public void Constructor_InvalidValue_ShouldThrowException(string accountId, string type, decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Movement(accountId, type, value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*Valor deve ser maior que zero*");
    }

    [Fact]
    public void IsCredit_WhenTypeIsC_ShouldReturnTrue()
    {
        // Arrange
        var movement = new Movement("acc123", "C", 100);

        // Act & Assert
        movement.IsCredit.Should().BeTrue();
        movement.IsDebit.Should().BeFalse();
    }

    [Fact]
    public void IsDebit_WhenTypeIsD_ShouldReturnTrue()
    {
        // Arrange
        var movement = new Movement("acc123", "D", 100);

        // Act & Assert
        movement.IsDebit.Should().BeTrue();
        movement.IsCredit.Should().BeFalse();
    }
}