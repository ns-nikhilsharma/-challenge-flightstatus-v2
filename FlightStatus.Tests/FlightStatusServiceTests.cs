using FlightStatus.Api.Models;
using FlightStatus.Api.Providers;
using FlightStatus.Api.Services;

namespace FlightStatus.Tests;

public class FlightStatusServiceTests
{
    private static readonly DateOnly TestDate = new(2026, 8, 19);

    [Fact]
    public async Task BothProviders_ReturnLaterUpdatedResult()
    {
        var providers = new IFlightStatusProvider[]
        {
            new FakeProvider("AeroTrack", new ProviderFlightStatus(
                "T100", TestDate, FlightStatus.OnTime,
                null, null, null, null, "T1", "A1", null,
                new DateTimeOffset(2026, 8, 19, 10, 0, 0, TimeSpan.Zero),
                "AeroTrack")),
            new FakeProvider("QuickFlight", new ProviderFlightStatus(
                "T100", TestDate, FlightStatus.Delayed,
                null, null, null, null, null, null, null,
                new DateTimeOffset(2026, 8, 19, 11, 0, 0, TimeSpan.Zero),
                "QuickFlight"))
        };

        var service = new FlightStatusService(providers);

        var result = await service.GetStatusAsync("T100", TestDate);

        Assert.Equal(FlightStatus.Delayed, result.Status);
        Assert.Equal("QuickFlight", result.Provider);
    }

    [Fact]
    public async Task OnlyOneProviderResponds_UsesThatResult()
    {
        var providers = new IFlightStatusProvider[]
        {
            new FakeProvider("AeroTrack", null),
            new FakeProvider("QuickFlight", new ProviderFlightStatus(
                "QF202", TestDate, FlightStatus.OnTime,
                null, null, null, null, null, null, null,
                DateTimeOffset.UtcNow, "QuickFlight"))
        };

        var service = new FlightStatusService(providers);

        var result = await service.GetStatusAsync("QF202", TestDate);

        Assert.Equal(FlightStatus.OnTime, result.Status);
        Assert.Equal("QuickFlight", result.Provider);
    }

    [Fact]
    public async Task NoProviderResponds_ReturnsUnknownWithMessage()
    {
        var service = new FlightStatusService(
        [
            new FakeProvider("AeroTrack", null),
            new FakeProvider("QuickFlight", null)
        ]);

        var result = await service.GetStatusAsync("ZZ999", TestDate);

        Assert.Equal(FlightStatus.Unknown, result.Status);
        Assert.Null(result.Provider);
        Assert.Contains("No usable flight status", result.Message);
    }

    [Fact]
    public async Task AeroTrackDetails_ArePreservedWhenSelected()
    {
        var expectedGate = "A12";

        var service = new FlightStatusService(
        [
            new FakeProvider("AeroTrack", new ProviderFlightStatus(
                "AR101", TestDate, FlightStatus.OnTime,
                null, null, null, null,
                "T1", expectedGate, null,
                DateTimeOffset.UtcNow, "AeroTrack")),
            new FakeProvider("QuickFlight", null)
        ]);

        var result = await service.GetStatusAsync("AR101", TestDate);

        Assert.Equal(expectedGate, result.Gate);
        Assert.Equal("T1", result.Terminal);
    }

    private sealed class FakeProvider(string name, ProviderFlightStatus? result)
        : IFlightStatusProvider
    {
        public string Name => name;

        public Task<ProviderFlightStatus?> GetStatusAsync(
            string flightNumber,
            DateOnly date,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }
}
