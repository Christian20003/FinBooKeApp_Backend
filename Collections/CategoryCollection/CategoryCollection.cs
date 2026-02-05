using System.Linq.Expressions;
using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.Models.Category;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Collections.CategoryCollection;

public class CategoryCollection(DataDbContext context)
    : DataCollection(context),
        ICategoryCollection
{
    private readonly DataDbContext _dbContext = context;

    public void CreateCategory(CategoryTag category)
    {
        _dbContext.Categories.Add(category);
    }

    public void UpdateCategory(CategoryTag category)
    {
        _dbContext.Categories.Update(category);
    }

    public void DeleteCategory(CategoryTag category)
    {
        _dbContext.Categories.Remove(category);
    }

    public async Task<CategoryTag?> GetCategory(Expression<Func<CategoryTag, bool>> condition)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(condition);
    }

    public async Task<IEnumerable<CategoryTag>> GetCategories(
        Expression<Func<CategoryTag, bool>> condition
    )
    {
        return await _dbContext.Categories.Where(condition).ToListAsync();
    }

    public async Task<Guid> GetUniqueId(Guid id)
    {
        var result = id;
        var trials = 10;
        while (
            await _dbContext.Categories.AnyAsync(category => category.Id == result) && trials >= 0
        )
        {
            if (trials == 0)
                throw new OperationCanceledException("Could not derive unique id for category");
            result = Guid.NewGuid();
        }
        return result;
    }
}
