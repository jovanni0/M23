namespace M23.Controller.Data;

using Microsoft.EntityFrameworkCore;



public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ProcessEventEntity> ProcessEvents => Set<ProcessEventEntity>();
}