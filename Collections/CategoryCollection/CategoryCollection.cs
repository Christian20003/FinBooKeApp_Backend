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

    public async Task<bool> ExistsCategoryId(Guid id)
    {
        return await _dbContext.Categories.AnyAsync(category => category.Id == id);
    }
}
