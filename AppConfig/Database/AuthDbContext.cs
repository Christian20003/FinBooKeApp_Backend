using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Database.Authentication;
using FinBookeAPI.Models.Token;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FinBookeAPI.AppConfig.Database;

public class AuthDbContext(
    DbContextOptions<AuthDbContext> options,
    IOptions<AuthDatabaseSettings> _settings
) : IdentityDbContext<UserAccount>(options)
{
    public DbSet<JwtToken> TokenCollection { get; init; }

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
        builder.Entity<JwtToken>().ToCollection("InvalidTokens");
    }
}
