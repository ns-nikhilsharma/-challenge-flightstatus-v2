using FlightStatus.Api.Models;

namespace FlightStatus.Api.Providers;

public sealed class AeroTrackProvider : IFlightStatusProvider
{
    public string Name => "AeroTrack";

    public Task<ProviderFlightStatus?> GetStatusAsync(
        string flightNumber,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var result = flightNumber.ToUpperInvariant() switch
        {
            "AR101" => Create(flightNumber, date, FlightStatus.OnTime, 90, 95, 210, 214,
                "T1", "A12", null, 180),
            "AR303" => Create(flightNumber, date, FlightStatus.Delayed, 120, 155, 270, 310,
                "T2", "B08", "Weather", 200),
            "AR505" => Create(flightNumber, date, FlightStatus.Diverted, 60, 65, 180, null,
                "T3", "C03", "Airport closure", 190),
            "QF606" => Create(flightNumber, date, FlightStatus.OnTime, 80, 82, 200, 204,
                "T1", "D10", null, 100),
            _ => null
        };

        return Task.FromResult(result);
    }

    private static ProviderFlightStatus Create(
        string flightNumber,
        DateOnly date,
        FlightStatus status,
        int scheduledDepartureMinutes,
        int actualDepartureMinutes,
        int scheduledArrivalMinutes,
        int? actualArrivalMinutes,
        string terminal,
        string gate,
        string? delayReason,
        int updatedMinutes)
    {
        var baseTime = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);

        return new ProviderFlightStatus(
            flightNumber,
            date,
            status,
            baseTime.AddMinutes(scheduledDepartureMinutes),
            baseTime.AddMinutes(actualDepartureMinutes),
            baseTime.AddMinutes(scheduledArrivalMinutes),
            actualArrivalMinutes.HasValue ? baseTime.AddMinutes(actualArrivalMinutes.Value) : null,
            terminal,
            gate,
            delayReason,
            baseTime.AddMinutes(updatedMinutes),
            "AeroTrack");
    }
}
