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
        EventId = LogEvents.CategoryCreateCategorySuccess,
        Level = LogLevel.Information,
        Message = "Category: Created new category successfully - UserId: {UserId}, Name: {Name}"
    )]
    private partial void LogCreateCategorySuccess(Guid userId, string name);

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
}
