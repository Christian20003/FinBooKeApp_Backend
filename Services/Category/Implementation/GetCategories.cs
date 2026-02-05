using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    public async Task<IEnumerable<CategoryTag>> GetCategories(Guid userId)
    {
        LogReadCategories(userId);

        if (userId == Guid.Empty)
        {
            LogInvalidUserId(userId);
            throw new ArgumentException("UserId is an empty Guid", nameof(userId));
        }

        var categories = await _collection.GetCategories(category => category.UserId == userId);
        var result = categories.Select(category => category.Copy());

        LogCategoriesRead(userId);

        return result;
    }

    public async Task<CategoryTag> GetCategory(Guid categoryId, Guid userId)
    {
        LogReadCategory(categoryId);

        var entity = await VerifyCategoryAccess(categoryId, userId);

        LogCategoryRead(entity);

        return entity.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryReadAll,
        Level = LogLevel.Information,
        Message = "Category: Read all categories from user - UserId: {UserId}"
    )]
    private partial void LogReadCategories(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryRead,
        Level = LogLevel.Information,
        Message = "Category: Read category - Id: {Id}"
    )]
    private partial void LogReadCategory(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryReadAllSuccess,
        Level = LogLevel.Information,
        Message = "Category: Read all categories from user - Id: {UserId}"
    )]
    private partial void LogCategoriesRead(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryReadSuccess,
        Level = LogLevel.Information,
        Message = "Category: Read category - Category: {Category}"
    )]
    private partial void LogCategoryRead(CategoryTag category);
}
