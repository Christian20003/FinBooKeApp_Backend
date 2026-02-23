using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Exceptions;
using Moq;

namespace FinBookeAPI.Tests.AmountManagement;

public partial class AmountManagementUnitTests
{
    [Fact]
    public async Task Should_CreateAmount_WhenDataIsValid()
    {
        await _service.CreateAmount(_amount);
        Assert.Contains(_database, item => item.Id == _amount.Id);
    }

    [Fact]
    public async Task Should_ReturnCreatedAmount_WhenDataIsValid()
    {
        var result = await _service.CreateAmount(_amount);

        Assert.Equal(_amount.Id, result.Id);
        Assert.Equal(_amount.UserId, result.UserId);
        Assert.Equal(_amount.PaymentTypeId, result.PaymentTypeId);
        Assert.Equal(_amount.CategoryId, result.CategoryId);
        Assert.Equal(_amount.Value, result.Value);
        Assert.Equal(_amount.Type, result.Type);
        Assert.Equal(_amount.Comment, result.Comment);
    }

    [Fact]
    public async Task Should_ReturnCopyOfCreatedAmount_WhenDataIsValid()
    {
        var result = await _service.CreateAmount(_amount);
        var entity = _database.First(e => e.Id == result.Id);

        Assert.NotSame(result, entity);
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenIdIsEmpty()
    {
        _amount.Id = Guid.Empty;
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenUserIdIsEmpty()
    {
        _amount.UserId = Guid.Empty;
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenPaymentTypeIdIsEmpty()
    {
        _amount.PaymentTypeId = Guid.Empty;
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenCategoryIdIsEmpty()
    {
        _amount.CategoryId = Guid.Empty;
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenValueIsNegative()
    {
        _amount.Value = -10;
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_FailCreatingAmount_WhenCommentIsTooLong()
    {
        _amount.Comment = new string('a', 1001);
        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_ThrowIdGenerationException_WhenIdGenerationFails()
    {
        _security
            .Setup(s => s.GenerateUniqueId(It.IsAny<Guid>(), It.IsAny<Func<Guid, Task<bool>>>()))
            .ThrowsAsync(new IdGenerationException("Id generation failed"));

        await Assert.ThrowsAsync<IdGenerationException>(() => _service.CreateAmount(_amount));
    }

    [Fact]
    public async Task Should_RethrowException_WhenDatabaseFails()
    {
        _collection.Setup(c => c.SaveChanges()).ThrowsAsync(new Exception("Database error"));
        await Assert.ThrowsAsync<Exception>(() => _service.CreateAmount(_amount));
    }
}
