using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.Category;
using FinBookeAPI.Services.Category;
using FinBookeAPI.Tests.Mocks.Collections;
using FinBookeAPI.Tests.Records;
using Moq;

namespace FinBookeAPI.Tests.Category;

public partial class CategoryServiceUnitTests
{
    private readonly Mock<ICategoryCollection> _collection;
    private readonly List<CategoryTag> _database;
    private readonly CategoryService _service;
    private CategoryTag _category;

    public CategoryServiceUnitTests()
    {
        _category = CategoryRecord.GetObject();
        _database = CategoryRecord.GetObjects();

        var logger = new Mock<ILogger<CategoryService>>();
        _collection = MockCategoryCollection.GetMock(_database);

        _service = new CategoryService(_collection.Object, logger.Object);
    }
}
