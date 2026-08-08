using IcMarkets.Domain;
using Microsoft.EntityFrameworkCore;

namespace IcMarkets.Infrastructure;

public class IcMarketsDbContext : DbContext
{
    //public IcMarketsDbContext()
    //{
    //}

    public IcMarketsDbContext(DbContextOptions<IcMarketsDbContext> options)
        : base(options)
    {
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    if (!optionsBuilder.IsConfigured)
    //    {
    //        optionsBuilder.UseNpgsql("Server=localhost:5432;Database=IcMarkets;Username=postgres;Password=postgres;");
    //    }
    //
    //    base.OnConfiguring(optionsBuilder);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blockchain>().HasKey(blockchain => new { blockchain.Pk });
        modelBuilder.Entity<Blockchain>(entity => entity.Property(x => x.CreatedAt).HasDefaultValueSql("now()"));

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Blockchain> Blockchains { get; set; }
}
