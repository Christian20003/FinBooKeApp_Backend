using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.Category;
using FinBookeAPI.Models.Configuration;

namespace FinBookeAPI.Services.Category;

public partial class CategoryService(
    ICategoryCollection collection,
    ILogger<CategoryService> logger
) : ICategoryService
{
    private readonly ICategoryCollection _collection = collection;
    private readonly ILogger<CategoryService> _logger = logger;

    [LoggerMessage(
        EventId = LogEvents.CategoryInvalidId,
        Level = LogLevel.Error,
        Message = "CategoryType: User id is invalid - UserId: {Id}"
    )]
    private partial void LogInvalidUserId(Guid id);

    [LoggerMessage(
        EventId = LogEvents.CategoryNotAccessible,
        Level = LogLevel.Error,
        Message = "CategoryType: Category is not accessible - Id: {Id}"
    )]
    private partial void LogNotAccessible(Guid id);
}
