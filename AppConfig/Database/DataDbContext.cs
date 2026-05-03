using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinBookeAPI.AppConfig.Database;

public class DataDbContext(
    DbContextOptions<DataDbContext> options,
    IOptions<FinanceDatabaseSettings> _settings
) : DbContext(options)
{
    public DbSet<Category> Categories { get; init; }

    public DbSet<Amount> Amounts { get; init; }

    public DbSet<PaymentMethod> PaymentMethods { get; init; }

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
        builder.Entity<Category>().ToCollection("categories");
        builder.Entity<Amount>().ToCollection("amounts");
        builder.Entity<PaymentMethod>().ToCollection("payment");
    }
}
