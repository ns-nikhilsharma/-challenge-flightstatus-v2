using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("local-ui", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddSingleton<IFlightStatusProvider, AeroTrackProvider>();
builder.Services.AddSingleton<IFlightStatusProvider, QuickFlightProvider>();
builder.Services.AddSingleton<FlightStatusService>();

var app = builder.Build();

app.UseCors("local-ui");

app.MapGet("/flights/status", async (
    string? flightNumber,
    string? date,
    FlightStatusService service,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(flightNumber) ||
        string.IsNullOrWhiteSpace(date))
    {
        return Results.BadRequest(new
        {
            message = "flightNumber and date are required."
        });
    }

    if (!DateOnly.TryParseExact(
            date,
            "yyyy-MM-dd",
            out var parsedDate))
    {
        return Results.BadRequest(new
        {
            message = "date must use yyyy-MM-dd format."
        });
    }

    var result = await service.GetStatusAsync(
        flightNumber.Trim().ToUpperInvariant(),
        parsedDate,
        cancellationToken);

    return Results.Ok(result);
});

app.Run();

public partial class Program;
