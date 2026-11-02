using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Core.Shared.Logging;
using Microsoft.Extensions.Logging;

namespace FinBooKeApp.Core.Services.Categories;

public partial class CategoryService : ICategoryService
{
    [LoggerMessage(
        EventId = LogEvents.CategoryCreateCategory,
        Level = LogLevel.Information,
        Message = "Category: Create new category - UserId: {UserId}, Name: {Name}"
    )]
    private partial void LogCreateCategory(Guid userId, string name);

    [LoggerMessage(
        EventId = LogEvents.CategoryGetCategory,
        Level = LogLevel.Information,
        Message = "Category: Get category - UserId: {UserId}, Id: {id}"
    )]
    private partial void LogGetCategory(Guid userId, Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryGetCategories,
        Level = LogLevel.Information,
        Message = "Category: Get categories - UserId: {UserId}"
    )]
    private partial void LogGetCategories(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryCreateCategorySuccess,
        Level = LogLevel.Information,
        Message = "Category: Created new category successfully - UserId: {UserId}, Name: {Name}"
    )]
    private partial void LogCreateCategorySuccess(Guid userId, string name);

    [LoggerMessage(
        EventId = LogEvents.CategoryGetCategorySuccess,
        Level = LogLevel.Information,
        Message = "Category: Get category successfully - UserId: {UserId}, Id: {id}"
    )]
    private partial void LogGetCategorySuccess(Guid userId, Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryGetCategoriesSucess,
        Level = LogLevel.Information,
        Message = "Category: Get categories successfully - UserId: {UserId}"
    )]
    private partial void LogGetCategoriesSuccess(Guid userId);

    [LoggerMessage(
        EventId = LogEvents.CategoryCreateCategoryFailure,
        Level = LogLevel.Information,
        Message = "Category: Validation failed to insert a category - UserId: {UserId}, Name: {Name}, ErrorCode: {ErrorCode}"
    )]
    private partial void LogCreateCategoryFailure(
        Guid userId,
        string name,
        CategoryServiceResultCode errorCode
    );

    [LoggerMessage(
        EventId = LogEvents.CategoryGetCategoryFailure,
        Level = LogLevel.Information,
        Message = "Category: Get category failure - UserId: {UserId}, Id: {id}, ErrorCode: {ErrorCode}"
    )]
    private partial void LogGetCategoryFailure(
        Guid userId,
        Guid id,
        CategoryServiceResultCode errorCode
    );
}
