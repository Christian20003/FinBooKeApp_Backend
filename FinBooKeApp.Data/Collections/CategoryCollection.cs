using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using MongoDB.Driver.Linq;

namespace FinBooKeApp.Data.Collections;

public class CategoryCollection(DataDbContext context) : ICategoryCollection
{
    private readonly DataDbContext _context = context;
    private readonly List<Category> _tracked = [];

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

    public void Insert(Category category)
    {
        _context.Categories.Add(category);
        _tracked.Add(category);
    }

    public List<Category> GetModifiedItems()
    {
        return [.. _tracked];
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
        _tracked.Clear();
    }

    public async Task<Category?> SelectAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Categories.FirstOrDefaultAsync(
            category => category.Id == id && category.UserId == userId,
            cancellationToken
        );
    }

    public async Task<IEnumerable<Category>> SelectByChildIds(
        IEnumerable<Guid> childrenIds,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Categories.Where(category =>
                category.Children.Any(id => childrenIds.Contains(id)) && category.UserId == userId
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsEntriesAsync(
        IEnumerable<Guid> ids,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        var result = true;
        foreach (var id in ids)
        {
            result &= await ExistEntryAsync(id, userId, cancellationToken);
            if (!result)
                break;
        }
        return result;
    }

    public async Task<bool> ExistEntryAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Categories.AnyAsync(
            item => item.Id == id && item.UserId == userId,
            cancellationToken
        );
    }

    public async Task<bool> ExistEntryAsync(
        string name,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context.Categories.AnyAsync(
            item => item.Name == name && item.UserId == userId,
            cancellationToken
        );
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
