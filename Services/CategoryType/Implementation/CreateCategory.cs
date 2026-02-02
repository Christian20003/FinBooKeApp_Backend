using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    public async Task<Category> CreateCategory(Category category)
    {
        LogCreateCategory(category);
        await VerifyNewCategory(category);
        _collection.CreateCategory(category);
        await _collection.SaveChanges();

        LogCategoryCreated(category);
        return new Category(category);
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryCreate,
        Level = LogLevel.Information,
        Message = "CategoryType: Create new category - {Category}"
    )]
    private partial void LogCreateCategory(Category category);

    [LoggerMessage(
        EventId = LogEvents.CategoryCreateSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Category created successfully - {Category}"
    )]
    private partial void LogCategoryCreated(Category category);
}
