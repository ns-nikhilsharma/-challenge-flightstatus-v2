namespace FlightStatus.Api.Models;

public enum FlightStatus
{
    OnTime,
    Delayed,
    Cancelled,
    Diverted,
    Unknown
}

public sealed record ProviderFlightStatus(
    string FlightNumber,
    DateOnly Date,
    FlightStatus Status,
    DateTimeOffset? ScheduledDepartureUtc,
    DateTimeOffset? ActualDepartureUtc,
    DateTimeOffset? ScheduledArrivalUtc,
    DateTimeOffset? ActualArrivalUtc,
    string? Terminal,
    string? Gate,
    string? DelayReason,
    DateTimeOffset LastUpdatedUtc,
    string Provider);

public sealed record FlightStatusResult(
    string FlightNumber,
    DateOnly Date,
    FlightStatus Status,
    DateTimeOffset? ScheduledDepartureUtc,
    DateTimeOffset? ActualDepartureUtc,
    DateTimeOffset? ScheduledArrivalUtc,
    DateTimeOffset? ActualArrivalUtc,
    string? Terminal,
    string? Gate,
    string? DelayReason,
    DateTimeOffset? LastUpdatedUtc,
    string? Provider,
    string? Message);
