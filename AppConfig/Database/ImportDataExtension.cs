using System.Text.Json;
using FinBookeAPI.Models.Authentication;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinBookeAPI.AppConfig.Database;

public static class ImportDataExtension
{
    /// <summary>
    /// This function can be extended to add further data into different
    /// collections.
    /// Therefore add a new line and adjust filename and DbSet.
    /// </summary>
    private static void LoadData(DataDbContext database, string path, ILogger logger)
    {
        AddToDatabase("Category.json", path, database.Categories, logger);
    }

    /// <summary>
    /// This function imports all users defined in a Users.json file.
    /// If the database contains already a user the import will be skipped.
    /// </summary>
    public async static Task<IServiceCollection> ImportUsers(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<DataImport>>();
        var manager = provider.GetRequiredService<IAccountManager>();
        var protector = provider.GetRequiredService<IDataProtection>();
        var factory = provider.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger("ImportUsers");

        var path = options.Value.Path + "Users.json";
        if (!options.Value.Import || await manager.GetUsersAsync().CountAsync() != 0)
            return services;
        if (!File.Exists(path))
        {
            logger.LogWarning("Given path {path} to user data does not exist", path);
            return services;
        }
        var json = File.ReadAllText(path);
        var users = JsonSerializer.Deserialize<UserAccount[]>(json) ?? [];

        foreach (var user in users)
        {
            user.UserName = protector.Protect(user.UserName ?? "");
            user.Email = protector.ProtectEmail(user.Email ?? "");
            var password = new string(user.PasswordHash) ?? "aF45%jh";
            user.PasswordHash = "";
            var result = await manager.CreateUserAsync(user, password);
            if (!result.Succeeded)
                logger.LogWarning("User could not be added");
        }

        logger.LogInformation("Users have been stored into the database");

        return services;
    }

    /// <summary>
    /// This function import all data from a corresponding .json file.
    /// </summary>
    public async static Task<IServiceCollection> ImportData(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<DataImport>>();
        var database = provider.GetRequiredService<DataDbContext>();
        var factory = provider.GetRequiredService<ILoggerFactory>();
        var logger = factory.CreateLogger("ImportData");

        if (!options.Value.Import)
            return services;
        LoadData(database, options.Value.Path, logger);
        await database.SaveChangesAsync();
        logger.LogInformation("Data have been stored into the database");
        return services;
    }

    /// <summary>
    /// This function reads the data from the given file and
    /// deserialize them to instances of class <c>T</c>.
    /// All identified objects will be tracked for storage
    /// into the database. Only if the corresponding collection
    /// cotains elements the tracking will be skipped.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the objects that will be stored in the database.
    /// </typeparam>
    /// <param name="fileName">
    /// The name of the json file where the data is stored.
    /// </param>
    /// <param name="path">
    /// The path where the file is stored.
    /// </param>
    /// <param name="collection">
    /// The database collection that will store the data.
    /// </param>
    /// <param name="logger">
    /// A logger to log state information.
    /// </param>
    private static void AddToDatabase<T>(
        string fileName,
        string path,
        DbSet<T> collection,
        ILogger logger
    )
        where T : class
    {
        if (collection.Any())
            return;
        var file = Path.Combine(path, fileName);
        if (!File.Exists(file))
        {
            logger.LogWarning("Given file {path} does not exist", file);
            return;
        }
        var json = File.ReadAllText(file);
        var data = JsonSerializer.Deserialize<T[]>(json) ?? [];
        foreach (var elem in data)
        {
            collection.Add(elem);
        }
    }
}
