using FinBookeAPI.Models.Exceptions;

namespace FinBookeAPI.Tests.CategoryType;

public partial class CategoryServiceUnitTests
{
    [Fact]
    public async Task Should_FailCreatingCategory_WhenIdIsEmpty()
    {
        _category.Id = Guid.Empty;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenIdAlreadyExist()
    {
        _category.Id = _database.First().Id;

        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _service.CreateCategory(_category)
        );
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenUserIdIsEmpty()
    {
        _category.UserId = Guid.Empty;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenNameIsEmpty()
    {
        _category.Name = "";

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenColorIsEmpty()
    {
        _category.Color = "";

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenColorHasInvalidFormat()
    {
        _category.Color = "abcde";

        await Assert.ThrowsAsync<FormatException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitAmountIsNegativ()
    {
        _category.Limit!.Amount = -30M;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitAmountIsZero()
    {
        _category.Limit!.Amount = 0M;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitPeriodIsNegativ()
    {
        _category.Limit!.PeriodDays = -3;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitPeriodIsZero()
    {
        _category.Limit!.PeriodDays = 0;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitAmountIsLargerThanFromParent()
    {
        var parent = _database.First();
        _category.Limit!.Amount = 500M;
        parent.Limit!.Amount = 44M;
        parent.Children = [_category.Id];
        parent.UserId = _category.UserId;

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenLimitAmountIsSmallerThanFromChildren()
    {
        var child = _database.First();
        child.Limit!.Amount = 500M;
        child.UserId = _category.UserId;
        _category.Limit!.Amount = 44M;
        _category.Children = [child.Id];

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenChildIdIsEmpty()
    {
        _category.Children = [Guid.Empty];

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenChildDoesNotExist()
    {
        _category.Children = [Guid.NewGuid()];

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_FailCreatingCategory_WhenChildIsNotOwned()
    {
        var child = _database.First();
        _category.Children = [child.Id];

        await Assert.ThrowsAsync<AuthorizationException>(() => _service.CreateCategory(_category));
    }

    [Fact]
    public async Task Should_AddCategoryToDatabase()
    {
        var parent = _database.First();
        parent.Children = [_category.Id];
        parent.UserId = _category.UserId;
        parent.Limit!.Amount = 600M;
        _category.Limit!.Amount = 20M;
        await _service.CreateCategory(_category);

        Assert.Contains(_database, elem => elem.Id == _category.Id);
    }

    [Fact]
    public async Task Should_ReturnInsertedCategory()
    {
        var result = await _service.CreateCategory(_category);

        Assert.Equal(_category.Id, result.Id);
        Assert.Equal(_category.Name, result.Name);
        Assert.Equal(_category.UserId, result.UserId);
        Assert.Equal(_category.Color, result.Color);
        Assert.Equal(_category.Limit!.Amount, result.Limit!.Amount);
        Assert.Equal(_category.Limit!.PeriodDays, result.Limit!.PeriodDays);
    }

    [Fact]
    public async Task Should_ReturnCopyOfInsertedCategory()
    {
        var result = await _service.CreateCategory(_category);
        var entity = _database.First(elem => elem.Id == result.Id);

        Assert.NotSame(result, entity);
    }

    [Fact]
    public async Task Should_AddCategory_WhenLimitIsNull()
    {
        _category.Limit = null;

        var result = await _service.CreateCategory(_category);

        Assert.Contains(_database, elem => elem.Id == _category.Id);
    }
}
