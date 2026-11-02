using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeApp.Data.Collections;

public class CategoryLimitCollection(DataDbContext context) : ICategoryLimitCollection
{
    private readonly DataDbContext _context = context;

    public async Task<List<CategoryLimit>> SelectByCategoryId(
        Guid categoryId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.Where(limit => limit.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CategoryLimit>> SelectByCategoryId(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.Where(limit => categoryIds.Contains(limit.CategoryId))
            .ToListAsync(cancellationToken);
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

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _context.ChangeTracker.Clear();
    }
}
