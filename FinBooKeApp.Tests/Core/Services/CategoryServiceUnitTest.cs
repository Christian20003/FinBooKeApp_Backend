using FinBooKeApp.Core.Services.Categories;
using FinBooKeApp.Core.Services.Categories.Models;
using FinBooKeApp.Data.Interfaces;
using FinBookeApp.Tests.Mocks.Collections.Categories;
using FinBooKeApp.Tests.Mocks.Collections.Categories;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinBooKeApp.Tests.Core.Services;

public class CategoryServiceUnitTest
{
    private readonly Mock<ICategoryCollection> _categoryCollection;
    private readonly Mock<ICategoryLimitCollection> _categoryLimitCollection;
    private readonly InMemoryCollection _database;
    private readonly CategoryService _service;

    public CategoryServiceUnitTest()
    {
        var logger = new Mock<ILogger<CategoryService>>();
        _database = new InMemoryCollection();
        _categoryCollection = MockCategoryCollection.GetMock(_database);
        _categoryLimitCollection = MockCategoryLimitCollection.GetMock(_database);
        _service = new CategoryService(
            _categoryCollection.Object,
            _categoryLimitCollection.Object,
            logger.Object
        );
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenCategoryNameIsAlreadyUsed_ReturnError()
    {
        var category = _database.GetParentCategory();
        var param = new CreateCategoryParams
        {
            UserId = category.UserId,
            Name = category.Name,
            Color = "#ffdd33",
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.False(result.HasValue());
        Assert.Equal(CategoryServiceResultCode.CATEGORY_ALREADY_EXIST, result.ErrorCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenCategoryCreatesCyclicDepedency_ReturnError()
    {
        var parent = _database.GetParentCategory();
        var child = _database.GetChildCategory();
        var param = new CreateCategoryParams
        {
            UserId = parent.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = child.Id,
            ChildrenIds = [parent.Id],
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.False(result.HasValue());
        Assert.Equal(CategoryServiceResultCode.INVALID_CATEGORY_DEPENDENCY, result.ErrorCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentCategoryIdDoesNotExist_ReturnError()
    {
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = Guid.NewGuid(),
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.False(result.HasValue());
        Assert.Equal(CategoryServiceResultCode.PARENT_CATEGORY_DOES_NOT_EXIST, result.ErrorCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenChildCategoryIdDoesNotExist_ReturnError()
    {
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [Guid.NewGuid()],
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.False(result.HasValue());
        Assert.Equal(CategoryServiceResultCode.CHILD_CATEGORY_DOES_NOT_EXIST, result.ErrorCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenChildCategoryHasLargerLimitAmount_ReturnError()
    {
        var child = _database.GetChildCategory();
        var childLimit = _database.GetLimit(child.Id);
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [child.Id],
            LimitAmount = childLimit.Amount - 5.0m,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.False(result.HasValue());
        Assert.Equal(CategoryServiceResultCode.INVALID_CATEGORY_LIMIT, result.ErrorCode);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenValidationSucceeded_StoreCategoryInDatabase()
    {
        var child = _database.GetChildCategory();
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [child.Id],
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        var dbItem = _database.Categories.FirstOrDefault(item => item.Name == param.Name);
        Assert.True(dbItem is not null);
        Assert.Equal(param.Name, dbItem.Name);
        Assert.Equal(param.Color, dbItem.Color);
        Assert.Equal(param.UserId, dbItem.UserId);
        Assert.Contains(dbItem.Children, id => id == child.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenValidationSucceeded_ReturnStoredCategory()
    {
        var child = _database.GetChildCategory();
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [child.Id],
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        Assert.Equal(param.Name, result.Value!.CreatedCategory.Name);
        Assert.Equal(param.Color, result.Value!.CreatedCategory.Color);
        Assert.Equal(param.UserId, result.Value!.CreatedCategory.UserId);
        Assert.Contains(result.Value!.CreatedCategory.Children, id => id == child.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSet_AddNewCategoryIdToParent()
    {
        var parent = _database.GetParentCategory();
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        Assert.Contains(result.Value!.CreatedCategory.Id, parent.Children);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSet_ReturnUpdatedParent()
    {
        var parent = _database.GetParentCategory();
        _database.TrackedCategories.Add(parent);
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        Assert.Contains(result.Value!.ModifiedCategories, item => item.Id == parent.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSetAndParentHasNewLimit_UpdateParentLimit()
    {
        var parent = _database.GetParentCategory();
        var parentLimit = _database.GetLimit(parent.Id);
        var oldAmount = parentLimit.Amount;
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
            LimitAmount = 300.0m,
        };

        _ = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.Equal(oldAmount + param.LimitAmount, parentLimit.Amount);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSetAndParentHasNewLimit_ReturnModifiedLimit()
    {
        var parent = _database.GetParentCategory();
        var parentLimit = _database.GetLimit(parent.Id);
        var oldAmount = parentLimit.Amount;
        _database.TrackedCategoryLimits.Add(parentLimit);
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
            LimitAmount = 300.0m,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.Contains(result.Value!.ModifiedCategoryLimits, item => item.Id == parentLimit.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSetAndParentDoesNotHaveAnyLimit_KeepParentLimitEmpty()
    {
        var parent = _database.GetParentCategory();
        var parentLimit = _database.GetLimit(parent.Id);
        _database.CategoryLimits.Remove(parentLimit);
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
            LimitAmount = 300.0m,
        };

        _ = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.DoesNotContain(_database.CategoryLimits, item => item.CategoryId == parent.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSetAndParentHasOldLimit_CreateNewParentLimit()
    {
        var parent = _database.GetParentCategory();
        var parentLimit = _database.GetLimit(parent.Id);
        parentLimit.Start = DateTime.UtcNow.AddMonths(-8);
        var oldAmount = parentLimit.Amount;
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
            LimitAmount = 300.0m,
        };

        _ = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.Equal(oldAmount, parentLimit.Amount);
        var newLimit = _database
            .CategoryLimits.OrderByDescending(item => item.CreatedAt)
            .First(item => item.CategoryId == parent.Id);
        Assert.Equal(oldAmount + param.LimitAmount, newLimit.Amount);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenParentIdIsSetAndParentHasOldLimit_ReturnNewCreatedParentLimit()
    {
        var parent = _database.GetParentCategory();
        var parentLimit = _database.GetLimit(parent.Id);
        parentLimit.Start = DateTime.UtcNow.AddMonths(-8);
        var oldAmount = parentLimit.Amount;
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ParentId = parent.Id,
            LimitAmount = 300.0m,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        var newLimit = _database
            .CategoryLimits.OrderByDescending(item => item.CreatedAt)
            .First(item => item.CategoryId == parent.Id);
        Assert.Contains(result.Value!.ModifiedCategoryLimits, item => item.Id == newLimit.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenChildIdIsSetAndChildHasAlreadyParent_DeleteChildFromPreviousParent()
    {
        var parent = _database.GetParentCategory();
        var child = _database.GetChildCategory();
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [child.Id],
        };

        _ = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.DoesNotContain(parent.Children, id => id == child.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenChildIdIsSetAndChildHasAlreadyParent_ReturnModifiedParent()
    {
        var parent = _database.GetParentCategory();
        var child = _database.GetChildCategory();
        _database.TrackedCategories.Add(parent);
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            ChildrenIds = [child.Id],
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.Contains(result.Value!.ModifiedCategories, item => item.Id == parent.Id);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenLimitAmountIsSet_StoreNewLimitAmountInDatabase()
    {
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            LimitAmount = 300.0m,
            LimitStartDate = DateTime.UtcNow,
            LimitTimePeriod = Data.Models.TimePeriod.ONE_MONTH,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        var newLimit = _database.GetLimit(result.Value!.CreatedCategory.Id);
        Assert.Equal(param.LimitAmount, newLimit.Amount);
        Assert.Equal(param.LimitStartDate, newLimit.Start);
        Assert.Equal(param.LimitTimePeriod, newLimit.Period);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenLimitAmountIsSet_ReturnNewLimitAmount()
    {
        var param = new CreateCategoryParams
        {
            UserId = _database.UserId,
            Name = "NewCategory",
            Color = "#ffdd33",
            LimitAmount = 300.0m,
            LimitStartDate = DateTime.UtcNow,
            LimitTimePeriod = Data.Models.TimePeriod.ONE_MONTH,
        };

        var result = await _service.CreateCategoryAsync(param, new CancellationToken());

        Assert.True(result.HasValue());
        Assert.Equal(param.LimitAmount, result.Value!.CreatedLimit!.Amount);
        Assert.Equal(param.LimitStartDate, result.Value!.CreatedLimit!.Start);
        Assert.Equal(param.LimitTimePeriod, result.Value!.CreatedLimit!.Period);
    }
}
