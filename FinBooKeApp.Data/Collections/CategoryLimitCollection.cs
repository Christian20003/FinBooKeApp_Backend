using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeApp.Data.Collections;

public class CategoryLimitCollection(DataDbContext context) : ICategoryLimitCollection
{
    private readonly DataDbContext _context = context;

    public async Task<IEnumerable<CategoryLimit>> SelectCurrentLimitByCategoryIdAsync(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    )
    {
        var latestLimits = await _context
            .CategoryLimits.Where(limit => categoryIds.Contains(limit.CategoryId))
            .OrderByDescending(limit => limit.CreatedAt)
            .ToListAsync(cancellationToken);

        return [.. latestLimits.GroupBy(limit => limit.CategoryId).Select(item => item.First())];
    }

    public async Task<CategoryLimit?> SelectCurrentLimitByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.OrderByDescending(limit => limit.CreatedAt)
            .Where(limit => limit.CategoryId == categoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Insert(CategoryLimit limit)
    {
        _context.CategoryLimits.Add(limit);
    }

    public List<CategoryLimit> GetModifiedItems()
    {
        return
        [
            .. _context
                .ChangeTracker.Entries<CategoryLimit>()
                .Where(item =>
                    item.State != EntityState.Detached && item.State != EntityState.Unchanged
                )
                .Select(item => item.Entity),
        ];
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task InsertAsync(CategoryLimit limit, CancellationToken cancellationToken)
    {
        _context.Add(limit);
        await _context.SaveChangesAsync(cancellationToken);
        //_context.CategoryLimits.Entry(limit).State = EntityState.Detached;
    }
}
