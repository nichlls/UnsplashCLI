using System.Net.Http.Headers;
using System.Text.Json;
using UnsplashCLI.Models.Unsplash;

namespace UnsplashCLI.Services;

public class UnsplashClient
{
    private readonly HttpClient _client;

    public UnsplashClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("Unsplash");
    }

    public async Task<UnsplashPhoto> GetRandomPhotoAsync()
    {
        try
        {
            using var response = await _client.GetAsync("photos/random");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var photo = await JsonSerializer.DeserializeAsync<UnsplashPhoto>(stream, _jsonOptions);

            if (photo == null)
            {
                throw new InvalidOperationException("Deserialisation returned null.");
            }

            return photo;
        }
        catch (Exception ex)
        {
            // TODO: Handle this properly
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<UnsplashPhoto>> SearchPhotosAsync(
        string query,
        int page = 1,
        int perPage = 10
    )
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search query cannot be null or empty.", nameof(query));
        }

        if (perPage < 1 || perPage > 30)
        {
            throw new ArgumentOutOfRangeException(
                nameof(perPage),
                "perPage must be between 1 and 30."
            );
        }

        try
        {
            // Build query string
            var url =
                $"search/photos?query={Uri.EscapeDataString(query)}&page={page}&per_page={perPage}";
            using var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var searchResult = await JsonSerializer.DeserializeAsync<UnsplashSearchResult>(
                stream,
                _jsonOptions
            );

            return searchResult?.Results ?? new List<UnsplashPhoto>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to search photos from Unsplash API: {ex.Message}", ex);
        }
        catch (JsonException ex)
        {
            throw new Exception(
                $"JSON deserialization error while searching photos: {ex.Message}",
                ex
            );
        }
        catch (Exception ex)
        {
            throw new Exception($"Unexpected error searching photos: {ex.Message}", ex);
        }
    }

    // Create json options, to so only once instance is created
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };
}
