using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinBooKeApp.Data.Configuration;

public class DataDbContext(
    DbContextOptions<DataDbContext> options,
    IOptions<FinanceDatabaseSettings> _settings
) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<CategoryLimit> CategoryLimits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var database = _settings.Value;
        var mongoClient = new MongoClient(database.ConnectionString);
        optionsBuilder.UseMongoDB(mongoClient, database.DatabaseName);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>().ToCollection("Category");
        builder.Entity<CategoryLimit>().ToCollection("CategoryLimit");
    }
}
