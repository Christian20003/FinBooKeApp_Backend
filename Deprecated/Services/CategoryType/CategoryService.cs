using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.CategoryType;

public partial class CategoryService(
    ICategoryCollection collection,
    ILogger<CategoryService> logger
) : ICategoryService
{
    private readonly ICategoryCollection _collection = collection;
    private readonly ILogger<CategoryService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.AuthenticationInvalidUserId,
        Level = LogLevel.Error,
        Message = "CategoryType: user id is invalid - {Id}"
    )]
    private partial void LogInvalidUserId(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryNotAccessible,
        Level = LogLevel.Error,
        Message = "CategoryType: Category is not accessible - {Category}"
    )]
    private partial void LogNotAccessibleCategory(Category category);
}
