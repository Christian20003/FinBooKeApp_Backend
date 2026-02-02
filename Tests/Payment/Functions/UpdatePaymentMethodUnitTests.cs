using System.ComponentModel.DataAnnotations;
using FinBookeAPI.Models.Exceptions;
using FinBookeAPI.Models.Payment;
using Newtonsoft.Json;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenIdIsEmpty()
    {
        var pm = _database.First().Copy();
        pm.Id = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenPaymentMethodDoesNotExist()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _paymentMethodService.UpdatePaymentMethod(_paymentMethod)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenUserIdIsEmpty()
    {
        var pm = _database.First().Copy();
        pm.UserId = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenTypeHasLessThan3Chars()
    {
        var pm = _database.First().Copy();
        pm.Type = "g";

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenTypeHasMoreThan100Chars()
    {
        var pm = _database.First().Copy();
        pm.Type = string.Concat(Enumerable.Repeat("x", 101));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenPaymentInstanceIdIsEmpty()
    {
        var pm = _database.First().Copy();
        pm.Instances = _paymentMethod.Instances;
        pm.Instances.First().Id = Guid.Empty;

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenPaymentInstanceNameHasLessThan3Chars()
    {
        var pm = _database.First().Copy();
        pm.Instances = _paymentMethod.Instances;
        pm.Instances.First().Name = "g";

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenPaymentInstanceNameHasMoreThan100Chars()
    {
        var pm = _database.First().Copy();
        pm.Instances = _paymentMethod.Instances;
        pm.Instances.First().Name = string.Concat(Enumerable.Repeat("x", 101));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_FailUpdatingPaymentMethod_WhenPaymentInstanceDescriptionHasMoreThan1000Chars()
    {
        var pm = _database.First().Copy();
        pm.Instances = _paymentMethod.Instances;
        pm.Instances.First().Name = string.Concat(Enumerable.Repeat("x", 1001));

        await Assert.ThrowsAsync<ValidationException>(
            () => _paymentMethodService.UpdatePaymentMethod(pm)
        );
    }

    [Fact]
    public async Task Should_StoreUpdateOnDatabase_WhenPaymentMethodDataIsValid()
    {
        var pm = _database.First().Copy();
        pm.Type = "Hello World";
        pm.Instances = _paymentMethod.Instances;

        var result = await _paymentMethodService.UpdatePaymentMethod(pm);
        var updatedpm = _database.First();

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(updatedpm);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Should_ReturnUpdatedPaymentMethod_WhenPaymentMethodDataIsValid()
    {
        var pm = _database.First().Copy();
        pm.Type = "Hello World";
        pm.Instances = _paymentMethod.Instances;

        var result = await _paymentMethodService.UpdatePaymentMethod(pm);

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Should_StoreCopyOfUpdatedPaymentMethod_WhenPaymentMethodDataIsValid()
    {
        var pm = _database.First().Copy();
        pm.Type = "Hello World";
        pm.Instances = _paymentMethod.Instances;

        var result = await _paymentMethodService.UpdatePaymentMethod(pm);
        var updatedpm = _database.First();

        Assert.NotSame(pm, updatedpm);
        Assert.NotSame(pm.Instances.First(), updatedpm.Instances.First());

        Assert.NotSame(result, updatedpm);
        Assert.NotSame(result.Instances.First(), updatedpm.Instances.First());
    }
}
