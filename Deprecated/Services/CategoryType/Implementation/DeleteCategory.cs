using FinBookeAPI.AppConfig.Documentation;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService : ICategoryService
{
    public async Task<Category> DeleteCategory(Guid categoryId, Guid userId)
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
                LogNotAccessibleCategory(parent);
                throw new AuthorizationException("Category parent is not accessible");
            }
            parent.Children = parent.Children.Where(childId => childId != categoryId);
            _collection.UpdateCategory(parent);
        }

        _collection.DeleteCategory(entity);
        await _collection.SaveChanges();

        LogCategoryDeleted(entity);

        return new Category(entity);
    }

    [LoggerMessage(
        EventId = LogEvents.CategoryDelete,
        Level = LogLevel.Information,
        Message = "CategoryType: Delete category - {Id}"
    )]
    private partial void LogDeleteCategory(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryDeleteSuccess,
        Level = LogLevel.Information,
        Message = "CategoryType: Category deleted successfully - {Category}"
    )]
    private partial void LogCategoryDeleted(Category category);
}
