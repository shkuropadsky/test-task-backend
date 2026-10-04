using Microsoft.EntityFrameworkCore;
using MinimalAPI.Domain;

namespace MinimalAPI.DataStorage;

public class BaseReportDbContext : DbContext
{
    public BaseReportDbContext(DbContextOptions options) : base(options) { }

    public DbSet<StatQuery> Queries => Set<StatQuery>();
    public DbSet<StatQueryResult> QueryResults => Set<StatQueryResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<StatQueryResult>(entity =>
        {
            entity.HasOne<StatQuery>()
                  .WithOne()
                  .HasForeignKey<StatQueryResult>(r => r.StatQueryId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => r.StatQueryId).IsUnique();
        });
    }
}
