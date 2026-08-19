using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;

namespace FlightStatus.Api.Services;

public sealed class FlightStatusService(IEnumerable<IFlightStatusProvider> providers)
{
    public async Task<FlightStatusResult> GetStatusAsync(
        string flightNumber,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var tasks = providers.Select(provider =>
            provider.GetStatusAsync(flightNumber, date, cancellationToken));

        var results = await Task.WhenAll(tasks);
        var usableResults = results.Where(result => result is not null).Cast<ProviderFlightStatus>().ToList();

        if (usableResults.Count == 0)
        {
            return new FlightStatusResult(
                flightNumber,
                date,
                FlightStatus.Unknown,
                null, null, null, null,
                null, null, null,
                null,
                null,
                "No usable flight status was returned by either provider.");
        }

        var selected = usableResults
            .OrderByDescending(result => result.LastUpdatedUtc)
            .First();

        return new FlightStatusResult(
            selected.FlightNumber,
            selected.Date,
            selected.Status,
            selected.ScheduledDepartureUtc,
            selected.ActualDepartureUtc,
            selected.ScheduledArrivalUtc,
            selected.ActualArrivalUtc,
            selected.Terminal,
            selected.Gate,
            selected.DelayReason,
            selected.LastUpdatedUtc,
            selected.Provider,
            null);
    }
}
