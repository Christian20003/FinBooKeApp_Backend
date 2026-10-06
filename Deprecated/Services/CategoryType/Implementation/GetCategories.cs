using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    public async Task<IEnumerable<Category>> GetCategories(Guid userId)
    {
        LogReadCategories(userId);

        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("UserId is an empty Guid", nameof(userId));
        }

        var categories = await _collection.GetCategories(category => category.UserId == userId);
        var result = categories.Select(category => new Category(category));

        LogCategoriesRead(userId);

        return result;
    }

    public async Task<Category> GetCategory(Guid categoryId, Guid userId)
    {
        LogReadCategory(categoryId);

        var entity = await VerifyCategoryAccess(categoryId, userId);

        LogCategoryRead(entity);

        return new Category(entity);
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryReadAll,
        Level = LogLevel.Information,
        Message = "CategoryType: Read all categories from user - {UserId}"
    )]
    private partial void LogReadCategories(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryRead,
        Level = LogLevel.Information,
        Message = "CategoryType: Read category - {Id}"
    )]
    private partial void LogReadCategory(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryReadAllSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Read all categories from user - {UserId}"
    )]
    private partial void LogCategoriesRead(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryReadSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Read category - {Category}"
    )]
    private partial void LogCategoryRead(Category category);
}
