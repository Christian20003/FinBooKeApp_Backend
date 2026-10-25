using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeApp.Data.Collections;

public class CategoryLimitCollection(DataDbContext context) : ICategoryLimitCollection
{
    private readonly DataDbContext _context = context;
    private readonly List<CategoryLimit> _tracked = [];

    public async Task<IEnumerable<CategoryLimit>> SelectCurrentLimitByCategoryId(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.OrderByDescending(limit => limit.CreatedAt)
            .GroupBy(limit => limit.CategoryId)
            .Select(item => item.First())
            .Where(limit => categoryIds.Contains(limit.CategoryId))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryLimit?> SelectCurrentLimitByCategoryId(
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
        _tracked.Add(limit);
    }

    public List<CategoryLimit> GetModifiedItems()
    {
        return [.. _tracked];
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _tracked.Clear();
    }

    public async Task InsertAsync(CategoryLimit limit, CancellationToken cancellationToken)
    {
        _context.Add(limit);
        await _context.SaveChangesAsync(cancellationToken);
        //_context.CategoryLimits.Entry(limit).State = EntityState.Detached;
    }
}
