using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public sealed class QuickFlightProvider : IFlightStatusProvider
{
    public string Name => "QuickFlight";

    public Task<ProviderFlightStatus?> GetStatusAsync(
        string flightNumber,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var result = flightNumber.ToUpperInvariant() switch
        {
            "AR101" => Create(flightNumber, date, FlightStatus.OnTime, 90, 210, 150),
            "QF202" => Create(flightNumber, date, FlightStatus.OnTime, 75, 195, 180),
            "QF404" => Create(flightNumber, date, FlightStatus.Cancelled, 100, 220, 160),
            "QF606" => Create(flightNumber, date, FlightStatus.Delayed, 80, 200, 220),
            _ => null
        };

        return Task.FromResult(result);
    }

    private static ProviderFlightStatus Create(
        string flightNumber,
        DateOnly date,
        FlightStatus status,
        int scheduledDepartureMinutes,
        int scheduledArrivalMinutes,
        int updatedMinutes)
    {
        var baseTime = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        return new ProviderFlightStatus(
            flightNumber,
            date,
            status,
            baseTime.AddMinutes(scheduledDepartureMinutes),
            null,
            baseTime.AddMinutes(scheduledArrivalMinutes),
            null,
            null,
            null,
            null,
            baseTime.AddMinutes(updatedMinutes),
            "QuickFlight");
    }
}
