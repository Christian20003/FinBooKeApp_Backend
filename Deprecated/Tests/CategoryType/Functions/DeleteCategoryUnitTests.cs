using FinBookeAPI.Models.CategoryType;
using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Tests.CategoryType;

public partial class CategoryServiceUnitTests
{
    [Fact]
    public async Task Should_FailRemovingCategory_WhenCategoryIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.DeleteCategory(Guid.Empty, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailRemovingCategory_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.DeleteCategory(Guid.NewGuid(), Guid.Empty)
        );
    }

    [Fact]
    public async Task Should_FailRemovingCategory_WhenCategoryDoesNotExist()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.DeleteCategory(_category.Id, _category.UserId)
        );
    }

    [Fact]
    public async Task Should_FailRemovingCategory_WhenCategoryIsNotOwned()
    {
        _database.Add(_category);

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.DeleteCategory(_category.Id, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailRemovingCategory_WhenParentIsNotOwned()
    {
        _database.First().Children = [.. _database.First().Children, _category.Id];
        _database.Add(_category);

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.DeleteCategory(_category.Id, _category.UserId)
        );
    }

    [Fact]
    public async Task Should_RemoveCategoryFromDatabase()
    {
        _database.Add(_category);

        await _service.DeleteCategory(_category.Id, _category.UserId);

        Assert.DoesNotContain(_database, elem => elem.Id == _category.Id);
    }

    [Fact]
    public async Task Should_RemoveCategoryIdFromParents_WhenCategoryIsDeleted()
    {
        var parent = _database.First();
        parent.UserId = _category.UserId;
        parent.Children = [_category.Id];
        _database.Add(_category);

        await _service.DeleteCategory(_category.Id, _category.UserId);

        Assert.DoesNotContain(parent.Children, id => id == _category.Id);
    }

    [Fact]
    public async Task Should_ReturnRemovedCategory()
    {
        _database.Add(_category);

        var result = await _service.DeleteCategory(_category.Id, _category.UserId);

        Assert.Equal(_category.Name, result.Name);
        Assert.Equal(_category.Color, result.Color);
        Assert.Equal(_category.Limit!.Amount, result.Limit!.Amount);
        Assert.Equal(_category.Limit!.PeriodDays, result.Limit!.PeriodDays);
        Assert.Equal(_category.Children, result.Children);
    }
}
