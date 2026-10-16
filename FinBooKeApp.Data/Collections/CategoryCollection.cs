using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeApp.Data.Collections;

public class CategoryCollection(DataDbContext context) : ICategoryCollection
{
    private readonly DataDbContext _context = context;

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken)
    {
        await foreach (var limit in _context.CategoryLimits.AsAsyncEnumerable())
        {
            if (limit.CategoryId == category.Id)
            {
                _context.CategoryLimits.Remove(limit);
            }
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task InsertAsync(Category category, CancellationToken cancellationToken)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Category?> SelectAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories.FirstOrDefaultAsync(
            category => category.Id == id,
            cancellationToken
        );
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
