using FinBookeAPI.AppConfig.Database;
using FinBookeAPI.Models.CategoryType;
using Microsoft.EntityFrameworkCore;

namespace FinBookeAPI.Collections.CategoryCollection;

public class CategoryCollection(DataDbContext context)
    : DataCollection(context),
        ICategoryCollection
{
    private readonly DataDbContext _dbContext = context;

    public void CreateCategory(Category category)
    {
        _dbContext.Categories.Add(category);
    }

    public void UpdateCategory(Category category)
    {
        _dbContext.Categories.Update(category);
    }

    public void DeleteCategory(Category category)
    {
        _dbContext.Categories.Remove(category);
    }

    public async Task<Category?> GetCategory(Func<Category, bool> condition)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(category => condition(category));
    }

    public async Task<IEnumerable<Category>> GetCategories(Func<Category, bool> condition)
    {
        return await _dbContext.Categories.Where(category => condition(category)).ToListAsync();
    }

    /* public async Task<Category?> HasParent(Guid categoryId, Guid userId)
    {
        var list = await GetCategories(userId);
        return list.FirstOrDefault(elem => elem.Children.Contains(categoryId));
    } */
}
