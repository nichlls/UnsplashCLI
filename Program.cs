using Microsoft.EntityFrameworkCore;
using UnsplashCLI.Data;
using UnsplashCLI.Endpoints;
using UnsplashCLI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Unsplash CLI API", Version = "v1" });
});

builder.Services.AddDbContext<PhotoDb>(option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"))
);

builder.Services.AddHttpClient(
    "Unsplash",
    client =>
    {
        client.BaseAddress = new Uri("https://api.unsplash.com/");
        client.DefaultRequestHeaders.Add("Accept-Version", "v1");
    }
);
builder.Services.AddScoped<UnsplashClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "hello");

app.MapPhotoEndpoints();

app.Run();
