using AnaBank.Transfers.Domain.Interfaces;
using System.Text.Json;
using System.Text;

namespace AnaBank.Transfers.Infrastructure.Clients;

public class AccountsClient : IAccountsClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AccountsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _baseUrl = "http://localhost:8081"; // Configurar via appsettings
    }

    public async Task DebitAsync(string accountId, decimal value, string authToken)
    {
        var request = new
        {
            Type = "D",
            Value = value
        };

        await MakeMovementAsync(request, authToken);
    }

    public async Task CreditAsync(string accountNumber, decimal value, string authToken)
    {
        var request = new
        {
            AccountNumber = accountNumber,
            Type = "C",
            Value = value
        };

        await MakeMovementAsync(request, authToken);
    }

    private async Task MakeMovementAsync(object request, string authToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/accounts/movements", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Tentar extrair o tipo de erro do ProblemDetails
            try
            {
                var problemDetails = JsonSerializer.Deserialize<ProblemDetailsResponse>(errorContent);
                throw new InvalidOperationException(problemDetails?.Type ?? "API_ERROR");
            }
            catch (JsonException)
            {
                throw new InvalidOperationException($"API_ERROR: {response.StatusCode}");
            }
        }
    }

    private class ProblemDetailsResponse
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public string? Detail { get; set; }
    }
}