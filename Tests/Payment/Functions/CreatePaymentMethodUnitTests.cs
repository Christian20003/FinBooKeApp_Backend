using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Exceptions;
using Newtonsoft.Json;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenIdIsEmpty()
    {
        _paymentMethod.Id = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenIdExist()
    {
        _paymentMethod.Id = _database.First().Id;

        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenUserIdIsEmpty()
    {
        _paymentMethod.UserId = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenTypeHasLessThan3Chars()
    {
        _paymentMethod.Type = "g";

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenTypeHasMoreThan100Chars()
    {
        _paymentMethod.Type = string.Concat(Enumerable.Repeat("x", 101));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenPaymentInstanceIdIsEmpty()
    {
        _paymentMethod.Instances.First().Id = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenPaymentInstanceNameHasLessThan3Chars()
    {
        _paymentMethod.Instances.First().Name = "g";

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenPaymentInstanceNameHasMoreThan100Chars()
    {
        _paymentMethod.Instances.First().Name = string.Concat(Enumerable.Repeat("x", 101));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailCreatingPaymentMethod_WhenPaymentInstanceDescriptionHasMoreThan1000Chars()
    {
        _paymentMethod.Instances.First().Description = string.Concat(Enumerable.Repeat("x", 1001));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.CreatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_StoreCreatedPaymentMethod_WhenDataIsValid()
    {
        var result = await _paymentMethodService.CreatePaymentMethod(_paymentMethod);

        Assert.Contains(_database, pm => pm.Id == result.Id);
    }

    [Fact]
    public async Task Should_StoreCopyOfCreatedPaymentMethod()
    {
        var result = await _paymentMethodService.CreatePaymentMethod(_paymentMethod);
        var stored = _database.First(pm => pm.Id == result.Id);

        Assert.NotSame(stored, result);
        foreach (var instance in result.Instances)
        {
            var pi = stored.Instances.First(elem => elem.Id == instance.Id);
            Assert.NotSame(pi, instance);
        }
    }

    [Fact]
    public async Task Should_ReturnCreatedPaymentMethod_WhenDataIsValid()
    {
        var result = await _paymentMethodService.CreatePaymentMethod(_paymentMethod);

        var expected = JsonConvert.SerializeObject(_paymentMethod);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }
}
