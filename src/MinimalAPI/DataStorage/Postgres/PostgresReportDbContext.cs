using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage.Postgres;

public class PostgresReportDbContext : BaseReportDbContext
{
    public PostgresReportDbContext(DbContextOptions<PostgresReportDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // уточнения для Postgres
        modelBuilder.Entity<StatQuery>().ToTable("queries");
        modelBuilder.Entity<StatQueryResult>().ToTable("query_results");
    }
}
