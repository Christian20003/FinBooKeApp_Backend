using FinBookeAPI.Collections.CategoryCollection;
using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Services.CategoryType;
using FinBookeAPI.Tests.Mocks.Collections;
using FinBookeAPI.Tests.Records;
using Moq;

namespace FinBookeAPI.Tests.CategoryType;

public partial class CategoryServiceUnitTests
{
    private readonly Mock<ICategoryCollection> _collection;
    private readonly List<Category> _database;
    private readonly CategoryService _service;
    private Category _category;

    public CategoryServiceUnitTests()
    {
        _category = CategoryRecord.GetObject();
        _database = CategoryRecord.GetObjects();

        var logger = new Mock<ILogger<CategoryService>>();
        _collection = MockCategoryCollection.GetMock(_database);

        _service = new CategoryService(_collection.Object, logger.Object);
    }
}
