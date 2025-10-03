using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
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
    (sp, client) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();
        var accessKey = config["Unsplash:AccessKey"];

        if (string.IsNullOrWhiteSpace(accessKey))
        {
            throw new InvalidOperationException("Unsplash API Access Key is missing.");
        }

        client.BaseAddress = new Uri("https://api.unsplash.com/");
        client.DefaultRequestHeaders.Add("Accept-Version", "v1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Client-ID",
            accessKey
        );
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

// Simple way of handling exceptions
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler =
            context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exceptionHandler != null)
        {
            var ex = exceptionHandler.Error;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = ex switch
                {
                    ArgumentException _ => StatusCodes.Status400BadRequest,
                    InvalidOperationException _ => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException _ => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError,
                },
                Title = ex.GetType().Name,
                Detail = app.Environment.IsDevelopment()
                    ? ex.Message
                    : "An unexpected error occurred.",
                Instance = context.Request.Path,
            };

            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    });
});

app.MapGet("/", () => "hello");

app.MapPhotoEndpoints();

app.Run();
