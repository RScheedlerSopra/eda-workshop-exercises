# EDA Workshop Exercises

.NET repository with NServiceBus containing exercises for my EDA (Event-Driven Architecture) workshop.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Project Structure

```
eda-workshop-exercises/
├── src/
│   └── EDAWorkshop.Api/        # ASP.NET Core Web API with NServiceBus
└── tests/
    └── EDAWorkshop.Tests/      # xUnit unit tests
```

## Running the Application

Navigate to the API project and start the application:

```bash
cd src/EDAWorkshop.Api
dotnet run
```

The API will start and be available at `https://localhost:7196` (or `http://localhost:5027`).

NServiceBus is configured with the [Learning Transport](https://docs.particular.net/transports/learning/), which stores messages as files on disk — ideal for local development and workshops.

### Available Endpoints

- `GET /weatherforecast` — Returns a sample weather forecast

## Running the Unit Tests

From the repository root, run:

```bash
dotnet test
```

To run only the tests project:

```bash
cd tests/EDAWorkshop.Tests
dotnet test
```

## Building the Solution

From the repository root:

```bash
dotnet build
```
