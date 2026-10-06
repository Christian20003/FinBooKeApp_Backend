using FinBookeAPI.AppConfig.Database;

namespace FinBookeAPI.Collections;

public class DataCollection(DataDbContext context) : IDataCollection
{
    private readonly DataDbContext _context = context;

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
