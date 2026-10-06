using FinBookeAPI.Models.Exceptions;
using Newtonsoft.Json;

namespace FinBookeAPI.Tests.Payment;

public partial class PaymentMethodServiceUnitTests
{
    [Fact]
    public async Task Should_FailGettingPaymentMethodById_WhenMethodIsNotAccessible()
    {
        var pm = _database.First();

        await Assert.ThrowsAsync<AuthorizationException>(
            () => _paymentMethodService.GetPaymentMethodById(pm.Id, _paymentMethod.UserId)
        );
    }

    [Fact]
    public async Task Should_FailGettingPaymentMethodById_WhenMethodDoesNotExist()
    {
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _paymentMethodService.GetPaymentMethodById(Guid.NewGuid(), _paymentMethod.UserId)
        );
    }

    [Fact]
    public async Task Should_ReturnCopyOfRequestedPaymentMethodById()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethodById(pm.Id, pm.UserId);

        Assert.NotSame(pm, result);
        foreach (var instance in result.Instances)
        {
            var pi = pm.Instances.First(elem => elem.Id == instance.Id);
            Assert.NotSame(pi, instance);
        }
    }

    [Fact]
    public async Task Should_ReturnRequestedPaymentMethod_WhenMethodIdIsParsed()
    {
        var pm = _database.First();
        var result = await _paymentMethodService.GetPaymentMethodById(pm.Id, pm.UserId);

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Should_ReturnRequestedPaymentMethod_WhenInstanceIdIsParsed()
    {
        var pm = _database.First(elem => elem.Instances.Count != 0);
        var result = await _paymentMethodService.GetPaymentMethodById(
            pm.Instances.First().Id,
            pm.UserId
        );

        var expected = JsonConvert.SerializeObject(pm);
        var actual = JsonConvert.SerializeObject(result);

        Assert.Equal(expected, actual);
    }
}
