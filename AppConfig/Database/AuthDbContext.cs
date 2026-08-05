using FinBookeAPI.Models.Configuration;
using FinBooKeAPI.Models.Database.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FinBookeAPI.AppConfig.Database;

public class AuthDbContext(
    DbContextOptions<AuthDbContext> options,
    IOptions<AuthDatabaseSettings> _settings
) : IdentityDbContext<UserAccount>(options)
{
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
    }
}
