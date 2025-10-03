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
            using var response = await _client.GetAsync("photos/randoms");
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

    // Create json options, to so only once instance is created
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true,
    };
}
