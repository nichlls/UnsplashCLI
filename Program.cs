using Microsoft.EntityFrameworkCore;
using UnsplashCLI.Data;
using UnsplashCLI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PhotoDb>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "hello");

app.MapGet(
    "/photos",
    async (PhotoDb db) =>
    {
        var photos = await db.Photos.ToListAsync();

        return photos;
    }
);
app.MapPut(
    "/photo",
    async (PhotoDb db, Photo photo) =>
    {
        await db.AddAsync(photo);
        await db.SaveChangesAsync();

        return Results.Ok($"Uploaded: {photo}");
    }
);

app.Run();
