# User Management API

A small ASP.NET Core Web API for creating, retrieving, updating, and deleting user records, backed by a SQLite database.

## What this is

A take-home implementation of a Web API that manages a single `Users` entity: create, get, update, and delete, backed by a single-table relational database. Built on .NET 8, ASP.NET Core, and EF Core with SQLite.

## How to run

**Prerequisites:** .NET 8 SDK, and Visual Studio 2022+ or the `dotnet` CLI.

1. Open `UserManagement.sln` (restores/builds automatically, or run `dotnet build UserManagement.sln`).
2. Run using the **http** launch profile (avoids a local HTTPS certificate prompt): select **http** from the launch profile dropdown in Visual Studio and press F5, or run `dotnet run --project UserManagement_API --launch-profile http`.
3. Swagger UI opens automatically at `/swagger` for testing all endpoints.

The SQLite database (`users.db`) is created automatically the first time you run the app.

**Tests:** `dotnet test UserManagement.sln`, or Test Explorer in Visual Studio.

## API endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get a single user by ID |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update an existing user (full replace) |
| DELETE | `/api/users/{id}` | Delete a user |

## Validation rules

- `FirstName`, `LastName`: required, max 100 characters
- `Email`: required, valid email format, max 256 characters, must be unique (`409 Conflict` on duplicate)
- `PhoneNumber`: required, valid phone format, max 20 characters
- `DateOfBirth`: cannot be in the future. Accepts standard date formats, e.g. `1990-05-15` (a full ISO 8601 timestamp also works, but only the date portion is used).

## Architecture

Four projects: **Application** (entity, DTOs, interfaces, service layer, and validation, with no external dependencies), **Infrastructure** (EF Core and SQLite repository), **API** (controllers, middleware, and DI wiring), and **Tests** (xUnit and Moq unit tests for the service layer).

`UserService` depends on `IUserRepository` rather than the concrete repository, so the business logic stays testable and doesn't know which database sits behind it. Requests and responses use DTOs instead of exposing the entity directly, which prevents over-posting. Data Annotations on the DTOs handle validation, and a single piece of middleware maps domain errors (`NotFoundException`, `ConflictException`) to HTTP status codes, which keeps the controllers thin.

