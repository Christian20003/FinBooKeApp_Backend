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
            .CategoryLimits.AsNoTracking()
            .Where(limit => limit.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CategoryLimit>> SelectByCategoryId(
        List<Guid> categoryIds,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.AsNoTracking()
            .Where(limit => categoryIds.Contains(limit.CategoryId))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryLimit?> SelectCurrentLimitByCategoryIdAsync(
        Guid categoryId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .CategoryLimits.AsNoTracking()
            .OrderByDescending(limit => limit.CreatedAt)
            .Where(limit => limit.CategoryId == categoryId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(CategoryLimit limit)
    {
        _context.CategoryLimits.Update(limit);
    }

    public void Insert(CategoryLimit limit)
    {
        _context.CategoryLimits.Add(limit);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
