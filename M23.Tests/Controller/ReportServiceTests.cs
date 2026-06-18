namespace M23.Tests.Controller;

using M23.Controller.Data;
using M23.Controller.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;



public class ReportServiceTests
{
    private static IDbContextFactory<AppDbContext> CreateFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContextFactory(options);
    }

    
    private static async Task SeedAsync(
        IDbContextFactory<AppDbContext> factory,
        IEnumerable<ProcessEventEntity> events)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.ProcessEvents.AddRange(events);
        await db.SaveChangesAsync();
    }

    
    [Fact]
    public async Task GetEventsAsync_NoFilter_ReturnsAll()
    {
        var factory = CreateFactory();
        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "M1", From = "Stopped", To = "Ready",   Timestamp = DateTime.UtcNow },
            new ProcessEventEntity { Source = "M3", From = "Stopped", To = "Running", Timestamp = DateTime.UtcNow }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetEventsAsync();

        Assert.Equal(2, result.Count);
    }

    
    [Fact]
    public async Task GetEventsAsync_FilterBySource_ReturnsOnlyMatching()
    {
        var factory = CreateFactory();
        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "M1",    From = "Stopped", To = "Ready",   Timestamp = DateTime.UtcNow },
            new ProcessEventEntity { Source = "SYSTEM", From = "Idle",   To = "Normal",  Timestamp = DateTime.UtcNow }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetEventsAsync(source: "M1");

        Assert.Single(result);
        Assert.Equal("M1", result[0].Source);
    }

    
    [Fact]
    public async Task GetEventsAsync_FilterByDateRange_ReturnsOnlyInRange()
    {
        var factory = CreateFactory();
        var now = DateTime.UtcNow;
        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "M1", From = "Stopped", To = "Ready", Timestamp = now.AddHours(-2) },
            new ProcessEventEntity { Source = "M1", From = "Stopped", To = "Ready", Timestamp = now }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetEventsAsync(from: now.AddHours(-1));

        Assert.Single(result);
    }

    
    [Fact]
    public async Task GetFaultPeriodsAsync_MatchedFaultAndRestart_ReturnsPeriodWithDuration()
    {
        var factory = CreateFactory();
        var faultTime   = DateTime.UtcNow.AddMinutes(-10);
        var restartTime = DateTime.UtcNow;

        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "SYSTEM", From = "Normal", To = "Fault", Timestamp = faultTime },
            new ProcessEventEntity { Source = "SYSTEM", From = "Fault",  To = "Idle",  Timestamp = restartTime }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetFaultPeriodsAsync();

        Assert.Single(result);
        Assert.Equal(faultTime, result[0].FaultedAt);
        Assert.Equal(restartTime, result[0].ResolvedAt);
        Assert.NotNull(result[0].Duration);
    }

    
    [Fact]
    public async Task GetFaultPeriodsAsync_UnresolvedFault_ReturnsPeriodWithNullDuration()
    {
        var factory = CreateFactory();
        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "SYSTEM", From = "Normal", To = "Fault", Timestamp = DateTime.UtcNow }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetFaultPeriodsAsync();

        Assert.Single(result);
        Assert.Null(result[0].ResolvedAt);
        Assert.Null(result[0].Duration);
    }

    
    [Fact]
    public async Task GetFaultPeriodsAsync_MultipleFaults_ReturnsAllPeriods()
    {
        var factory = CreateFactory();
        var now = DateTime.UtcNow;
        await SeedAsync(factory,
        [
            new ProcessEventEntity { Source = "SYSTEM", From = "Normal", To = "Fault", Timestamp = now.AddMinutes(-20) },
            new ProcessEventEntity { Source = "SYSTEM", From = "Fault",  To = "Idle",  Timestamp = now.AddMinutes(-15) },
            new ProcessEventEntity { Source = "SYSTEM", From = "Normal", To = "Fault", Timestamp = now.AddMinutes(-5) },
            new ProcessEventEntity { Source = "SYSTEM", From = "Fault",  To = "Idle",  Timestamp = now }
        ]);

        var service = new ReportService(factory);
        var result = await service.GetFaultPeriodsAsync();

        Assert.Equal(2, result.Count);
    }
}



public class TestDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    
    public TestDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    
    public AppDbContext CreateDbContext() => new(_options);
}