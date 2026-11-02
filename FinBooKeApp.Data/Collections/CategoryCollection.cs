using FinBooKeApp.Data.Configuration;
using FinBooKeApp.Data.Interfaces;
using FinBooKeApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBooKeApp.Data.Collections;

public class CategoryCollection(DataDbContext context) : ICategoryCollection
{
    private readonly DataDbContext _context = context;

    public void Insert(Category category)
    {
        _context.Categories.Add(category);
    }

    public void Update(Category category)
    {
        _context.Categories.Update(category);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Category>> SelectAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context
            .Categories.AsNoTracking()
            .Where(category => category.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> SelectAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Categories.AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Id == id && category.UserId == userId,
                cancellationToken
            );
    }

    public async Task<Category?> SelectAsync(
        string name,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Categories.AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Name == name && category.UserId == userId,
                cancellationToken
            );
    }

    public async Task<Category?> SelectParentAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Categories.AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Children.Contains(id) && category.UserId == userId,
                cancellationToken
            );
    }
}
