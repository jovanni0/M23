namespace M23.Controller.Services;

using M23.Controller.Data;
using Microsoft.EntityFrameworkCore;



public record EventRecord(
    string Source,
    string From,
    string To,
    DateTime Timestamp);



public record FaultPeriod(
    DateTime FaultedAt,
    DateTime? ResolvedAt,
    TimeSpan? Duration);



public class ReportService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    
    public ReportService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    
    public async Task<List<EventRecord>> GetEventsAsync(
        DateTime? from = null,
        DateTime? to = null,
        string? source = null)
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var query = db.ProcessEvents.AsQueryable();

        if (from.HasValue)
            query = query.Where(e => e.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.Timestamp <= to.Value);

        if (!string.IsNullOrWhiteSpace(source))
            query = query.Where(e => e.Source == source);

        return await query
            .OrderByDescending(e => e.Timestamp)
            .Select(e => new EventRecord(e.Source, e.From, e.To, e.Timestamp))
            .ToListAsync();
    }

    
    public async Task<List<FaultPeriod>> GetFaultPeriodsAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();

        var faultEvents = await db.ProcessEvents
            .Where(e => e.Source == "SYSTEM" &&
                       (e.To == "Fault" || e.To == "Idle"))
            .OrderBy(e => e.Timestamp)
            .ToListAsync();

        var periods = new List<FaultPeriod>();
        DateTime? faultStart = null;

        foreach (var e in faultEvents)
        {
            if (e.To == "Fault")
            {
                faultStart = e.Timestamp;
            }
            else if (e.To == "Idle" && faultStart.HasValue)
            {
                var resolved = e.Timestamp;
                periods.Add(new FaultPeriod(
                    faultStart.Value,
                    resolved,
                    resolved - faultStart.Value));
                faultStart = null;
            }
        }

        if (faultStart.HasValue)
            periods.Add(new FaultPeriod(faultStart.Value, null, null));

        return periods;
    }
}