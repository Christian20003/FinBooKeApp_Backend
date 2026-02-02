using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Tests.CategoryType;

public partial class CategoryServiceUnitTests
{
    [Fact]
    public async Task Should_FailReadingCategories_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetCategories(Guid.Empty));
    }

    [Fact]
    public async Task Should_ReturnRequestedCategories()
    {
        var entity = _database.FirstOrDefault(elem => elem.Children.Any());
        var entities = _database.Where(elem => elem.UserId == entity!.UserId);
        var result = await _service.GetCategories(entity!.UserId);

        Assert.All(result, elem => entities.Contains(elem));
    }

    [Fact]
    public async Task Should_ReturnCopyOfRequestedCategories()
    {
        var entity = _database.FirstOrDefault(elem => elem.Children.Any());
        var entities = _database.Where(elem => elem.UserId == entity!.UserId);
        var result = await _service.GetCategories(entity!.UserId);

        foreach (var category in result)
        {
            var fromDatabase = _database.First(elem => elem.Id == category.Id);
            Assert.NotSame(fromDatabase, category);
        }
    }

    [Fact]
    public async Task Should_FailReadingCategory_WhenCategoryIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetCategory(Guid.Empty, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailReadingCategory_WhenUserIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetCategory(Guid.NewGuid(), Guid.Empty)
        );
    }

    [Fact]
    public async Task Should_FailReadingCategory_WhenCategoryDoesNotExist()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _service.GetCategory(Guid.NewGuid(), Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_FailReadingCategory_WhenCategoryIsNotOwned()
    {
        await Assert.ThrowsAsync<AuthorizationException>(
            () => _service.GetCategory(_database.First().Id, Guid.NewGuid())
        );
    }

    [Fact]
    public async Task Should_ReturnRequestedCategory()
    {
        var entity = _database.First();
        var result = await _service.GetCategory(entity.Id, entity.UserId);

        Assert.Equal(entity.Name, result.Name);
        Assert.Equal(entity.Color, result.Color);
        Assert.Equal(entity.Limit!.Amount, result.Limit!.Amount);
        Assert.Equal(entity.Limit!.PeriodDays, result.Limit!.PeriodDays);
        Assert.Equal(entity.Children, result.Children);
    }

    [Fact]
    public async Task Should_ReturnCopyOfRequestedCategory()
    {
        var entity = _database.First();
        var result = await _service.GetCategory(entity.Id, entity.UserId);

        Assert.NotSame(entity, result);
    }
}
