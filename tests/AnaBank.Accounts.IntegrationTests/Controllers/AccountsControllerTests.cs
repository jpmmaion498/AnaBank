using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;
using AnaBank.Accounts.API.Controllers;
using AnaBank.Accounts.Application.Commands.RegisterAccount;
using AnaBank.Accounts.Application.Commands.Login;

namespace AnaBank.Accounts.IntegrationTests.Controllers;

public class AccountsControllerTests : IClassFixture<AccountsApiWebApplicationFactory>
{
    private readonly AccountsApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AccountsControllerTests(AccountsApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ValidAccount_ShouldReturn201()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();
        
        var request = new RegisterAccountRequest("Maria Santos", "11144477735", "password123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<RegisterAccountResult>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Number.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_InvalidCpf_ShouldReturn400()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();
        
        var request = new RegisterAccountRequest("Invalid User", "12345678900", "password123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var problemDetails = await response.Content.ReadAsStringAsync();
        problemDetails.Should().Contain("INVALID_DOCUMENT");
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturn200WithToken()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();
        
        var registerRequest = new RegisterAccountRequest("Test User", "11144477735", "password123");
        await _client.PostAsJsonAsync("/api/accounts", registerRequest);

        var loginRequest = new LoginRequest("11144477735", "password123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturn401()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();
        
        var loginRequest = new LoginRequest("11144477735", "wrongpassword");

        // Act
        var response = await _client.PostAsJsonAsync("/api/accounts/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBalance_WithoutToken_ShouldReturn401()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();

        // Act
        var response = await _client.GetAsync("/api/accounts/balance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBalance_WithValidToken_ShouldReturn200()
    {
        // Arrange
        await _factory.InitializeDatabaseAsync();
        
        var registerRequest = new RegisterAccountRequest("Test User", "11144477735", "password123");
        await _client.PostAsJsonAsync("/api/accounts", registerRequest);

        var loginRequest = new LoginRequest("11144477735", "password123");
        var loginResponse = await _client.PostAsJsonAsync("/api/accounts/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResult>();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await _client.GetAsync("/api/accounts/balance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}