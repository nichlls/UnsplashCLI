# UnsplashCLI

A command-line interface application for interacting with the Unsplash API (wip), featuring a PostgreSQL database for storing photo metadata.

## Features

- REST API endpoints for photo management
- PostgreSQL database integration using Entity Framework Core
- Photo storage with metadata (name, author, image URL)
- Built with .NET 8 and ASP.NET Core

## Technology Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Npgsql (PostgreSQL provider)

## Getting Started

Either use the devcontainer or:

1. Ensure you have .NET 8 SDK installed
2. Configure your PostgreSQL connection string in `appsettings.Development.json`
3. Run the application with `dotnet run`
4. Access the API at the configured endpoints

## API Endpoints

- (wip)

## Database

The application uses Entity Framework Core migrations for database schema management. Run `dotnet ef database update` to apply migrations.
