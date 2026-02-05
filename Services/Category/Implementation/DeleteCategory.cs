using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService : ICategoryService
{
    public async Task<CategoryTag> DeleteCategory(Guid categoryId, Guid userId)
    {
        LogDeleteCategory(categoryId);

        var entity = await VerifyCategoryAccess(categoryId, userId);
        var parent = await _collection.GetCategory(category =>
            category.Children.Contains(categoryId)
        );
        if (parent is not null)
        {
            if (parent.UserId != userId)
            {
                LogNotAccessible(parent.Id);
                throw new AuthorizationException("Category parent is not accessible");
            }
            parent.Children = [.. parent.Children.Where(childId => childId != categoryId)];
            _collection.UpdateCategory(parent);
        }

        _collection.DeleteCategory(entity);
        await _collection.SaveChanges();

        LogCategoryDeleted(entity);

        return entity.Copy();
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryDelete,
        Level = LogLevel.Information,
        Message = "Category: Delete category - Id: {Id}"
    )]
    private partial void LogDeleteCategory(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryDeleteSuccess,
        Level = LogLevel.Information,
        Message = "Category: Category deleted successfully - Category: {Category}"
    )]
    private partial void LogCategoryDeleted(CategoryTag category);
}
