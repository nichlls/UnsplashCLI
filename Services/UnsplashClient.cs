using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using UnsplashCLI.Models.Unsplash;

namespace UnsplashCLI.Services;

public class UnsplashClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    // TODO: Implement configuration (appsettings/)
    private readonly IConfiguration _configuration;

    public UnsplashClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<UnsplashPhoto?> GetRandomPhotoAsync(string? accessKeyFromQuery)
    {
        // TODO: Use configuration
        var key = accessKeyFromQuery;
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        var client = _httpClientFactory.CreateClient("Unsplash");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Client-ID",
            key
        );

        using var response = await client.GetAsync("photos/random");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.Error.WriteLine($"Unsplash API error: {response.StatusCode} - {error}");
            return null;
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        var photo = await JsonSerializer.DeserializeAsync<UnsplashPhoto>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        return photo;
    }
}
