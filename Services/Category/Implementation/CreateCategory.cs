using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    public async Task<CategoryTag> CreateCategory(CategoryTag category)
    {
        LogCreateCategory(category);

        await VerifyCategory(category);
        category.Id = await _security.GenerateUniqueId(category.Id, _collection.ExistsCategoryId);
        _collection.CreateCategory(category);
        await _collection.SaveChanges();

        LogCategoryCreated(category);
        return category.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryCreate,
        Level = LogLevel.Information,
        Message = "Category: Create new category - Category: {Category}"
    )]
    private partial void LogCreateCategory(CategoryTag category);

    [LoggerMessage(
        EventId = LogEvents.CategoryCreateSuccess,
        Level = LogLevel.Information,
        Message = "Category: Category created successfully - Category: {Category}"
    )]
    private partial void LogCategoryCreated(CategoryTag category);
}
