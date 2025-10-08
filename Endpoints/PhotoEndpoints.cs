using Microsoft.EntityFrameworkCore;
using UnsplashCLI.Data;
using UnsplashCLI.Models;
using UnsplashCLI.Services;

namespace UnsplashCLI.Endpoints;

public static class PhotoEndpoints
{
    public static void MapPhotoEndpoints(this WebApplication app)
    {
        app.MapGet("/photos", async (PhotoDb db) => await db.Photos.ToListAsync());

        // Get photos by author
        app.MapGet(
            "/photos/by-author/{author}",
            async (PhotoDb db, string author) =>
                await db.Photos.Where(p => p.Author == author).ToListAsync()
        );

        // Delete photo by ID
        app.MapDelete(
            "/photos/{id:int}",
            async (PhotoDb db, int id) =>
            {
                var photo = await db.Photos.FindAsync(id);
                if (photo == null)
                {
                    return Results.NotFound($"Photo with ID {id} not found.");
                }

                db.Photos.Remove(photo);
                await db.SaveChangesAsync();
                return Results.Ok($"Deleted photo with ID {id}");
            }
        );

        // Fetch random photo and save it
        app.MapPost(
            "/unsplash/import-random",
            async (PhotoDb db, UnsplashClient unsplash) =>
            {
                var response = await unsplash.GetRandomPhotoAsync();
                if (response == null)
                {
                    return Results.Problem("Failed to fetch random photo from Unsplash.");
                }

                var toSave = new Photo
                {
                    Name = response.Description ?? response.AltDescription,
                    Author = response.User?.Name ?? response.User?.Username ?? "Unknown author",
                    ImageURL =
                        response.Urls?.Regular ?? response.Urls?.Full ?? response.Urls?.Small,
                };

                await db.Photos.AddAsync(toSave);
                await db.SaveChangesAsync();

                return Results.Ok(toSave);
            }
        );

        // Search photos by query & import up to 'count' photos
        app.MapPost(
            "/unsplash/import-search",
            async (PhotoDb db, UnsplashClient unsplash, string query, int count = 1) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Results.BadRequest("Query parameter is required.");
                }

                if (count < 1 || count > 10)
                {
                    return Results.BadRequest("Count must be between 1 and 10.");
                }

                try
                {
                    var results = await unsplash.SearchPhotosAsync(query, perPage: count);

                    if (results.Count == 0)
                    {
                        return Results.NotFound($"No photos found for query '{query}'.");
                    }

                    var savedPhotos = new List<Photo>();

                    foreach (var response in results.Take(count))
                    {
                        var toSave = new Photo
                        {
                            Name = response.Description ?? response.AltDescription ?? "Untitled",
                            Author =
                                response.User?.Name ?? response.User?.Username ?? "Unknown author",
                            ImageURL =
                                response.Urls?.Regular
                                ?? response.Urls?.Full
                                ?? response.Urls?.Small
                                ?? string.Empty,
                        };

                        await db.Photos.AddAsync(toSave);
                        savedPhotos.Add(toSave);
                    }

                    await db.SaveChangesAsync();

                    return Results.Ok(savedPhotos);
                }
                catch (Exception ex)
                {
                    return Results.Problem($"Failed to import searched photos: {ex.Message}");
                }
            }
        );
    }
}
