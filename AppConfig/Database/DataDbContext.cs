using FinBookeAPI.Models.AmountManagement;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinBookeAPI.AppConfig.Database;

/// <summary>
/// This class models the context for the financial data database.
/// </summary>
/// <param name="options">
/// The options to be used by a DbContext.
/// </param>
/// <param name="_settings">
/// The settings including all necessary information from the <c>appsettings.json</c> file.
/// </param>
public class DataDbContext(
    DbContextOptions<DataDbContext> options,
    IOptions<FinancialDataDtabaseSettings> _settings
) : DbContext(options)
{
    public DbSet<Category> Categories { get; init; }

    public DbSet<Amount> Amounts { get; init; }

    public DbSet<PaymentMethod> PaymentMethods { get; init; }

    /// <summary>
    /// This function configures the database storing financial data by reading all specified configurations
    /// in the <c>appsettings.json</c> file.
    /// </summary>
    /// <param name="optionsBuilder">
    /// API to configure the DbContext for EF-Core.
    /// </param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var database = _settings.Value;
        var mongoClient = new MongoClient(database.ConnectionString);
        optionsBuilder.UseMongoDB(mongoClient, database.DatabaseName);
    }

    /// <summary>
    /// This function creates all specified tables / collections in the database.
    /// </summary>
    /// <param name="builder">
    ///  API to model the shape of all entities in this database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Category>().ToCollection("categories");
        builder.Entity<Amount>().ToCollection("amounts");
        builder.Entity<PaymentMethod>().ToCollection("payment");
    }
}
