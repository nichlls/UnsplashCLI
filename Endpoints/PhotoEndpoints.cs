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
        // Usage: POST /unsplash/import-random?accessKey=ACCESS_KEY
        app.MapPost(
            "/unsplash/import-random",
            async (PhotoDb db, UnsplashClient unsplash, string? accessKey) =>
            {
                var response = await unsplash.GetRandomPhotoAsync(accessKey);
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
    }
}
