using AnaBank.Transfers.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AnaBank.Transfers.UnitTests.Domain.Entities;

public class TransferTests
{
    [Fact]
    public void Constructor_ValidParameters_ShouldCreateTransfer()
    {
        // Arrange
        var originAccountId = "acc123";
        var destinationAccountId = "acc456";
        var value = 100.50m;

        // Act
        var transfer = new Transfer(originAccountId, destinationAccountId, value);

        // Assert
        transfer.OriginAccountId.Should().Be(originAccountId);
        transfer.DestinationAccountId.Should().Be(destinationAccountId);
        transfer.Value.Should().Be(value);
        transfer.Status.Should().Be("COMPLETED");
        transfer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("", "acc456", 100)]
    [InlineData("   ", "acc456", 100)]
    [InlineData(null, "acc456", 100)]
    public void Constructor_InvalidOriginAccountId_ShouldThrowException(string originAccountId, string destinationAccountId, decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Transfer(originAccountId, destinationAccountId, value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*origem*");
    }

    [Theory]
    [InlineData("acc123", "", 100)]
    [InlineData("acc123", "   ", 100)]
    [InlineData("acc123", null, 100)]
    public void Constructor_InvalidDestinationAccountId_ShouldThrowException(string originAccountId, string destinationAccountId, decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Transfer(originAccountId, destinationAccountId, value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*destino*");
    }

    [Fact]
    public void Constructor_SameOriginAndDestination_ShouldThrowException()
    {
        // Arrange
        var accountId = "acc123";

        // Act & Assert
        FluentActions.Invoking(() => new Transfer(accountId, accountId, 100))
            .Should().Throw<ArgumentException>()
            .WithMessage("*iguais*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_InvalidValue_ShouldThrowException(decimal value)
    {
        // Act & Assert
        FluentActions.Invoking(() => new Transfer("acc123", "acc456", value))
            .Should().Throw<ArgumentException>()
            .WithMessage("*maior que zero*");
    }

    [Fact]
    public void MarkAsFailed_ShouldUpdateStatus()
    {
        // Arrange
        var transfer = new Transfer("acc123", "acc456", 100);

        // Act
        transfer.MarkAsFailed();

        // Assert
        transfer.Status.Should().Be("FAILED");
    }

    [Fact]
    public void MarkAsReversed_ShouldUpdateStatus()
    {
        // Arrange
        var transfer = new Transfer("acc123", "acc456", 100);

        // Act
        transfer.MarkAsReversed();

        // Assert
        transfer.Status.Should().Be("REVERSED");
    }
}